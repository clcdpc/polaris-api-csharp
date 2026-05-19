# Polaris API (PAPI) Coding Agent Reference

Source: *Polaris API Reference 7.7*.

This is a practical implementation guide for agents and developers integrating with Polaris REST API (PAPI). It summarizes the request patterns, authentication/signature rules, response conventions, and the most commonly used endpoints.

> Use the vendor reference for exhaustive field lists and edge cases. This guide is optimized for coding tasks and API-client implementation.

---

## 1. Base URL and Route Shape

Most examples follow this pattern:

```text
https://{hostname}/PAPIService/REST/{scope}/{version}/{langID}/{appID}/{orgID}/{resource}
```

Common route values:

| Segment | Meaning | Common value |
|---|---|---|
| `scope` | API area | `public` |
| `version` | API version | `v1` or `v2` depending on endpoint |
| `langID` | Label language | `1033` for English |
| `appID` | Calling application ID | `100` unless otherwise assigned |
| `orgID` | Organization ID | `1` for system, or branch org ID |

Example:

```text
https://example.org/PAPIService/REST/public/v1/1033/100/1/api
```

---

## 2. Transport, Content Types, and Dates

### HTTPS / SSL

Protected endpoints require HTTPS. If `SecurityMode` is set to `Transport`, all PAPI calls, including public calls, must use HTTPS.

### JSON vs XML

PAPI supports XML and JSON. XML is the default.

For JSON requests and responses:

```http
Content-Type: application/json
Accept: application/json
```

For XML:

```http
Content-Type: application/xml
Accept: application/xml
```

When a request body is present, include `Content-Length`.

### Date Formats

The HTTP date used for request signing must be RFC1123 and within ±30 minutes of current server time:

```text
Wed, 17 Oct 2012 22:23:32 GMT
```

If the client cannot set the standard `Date` header, use:

```http
PolarisDate: Wed, 17 Oct 2012 22:23:32 GMT
```

JSON date fields may use either:

```json
"2022-01-04T14:58:32-0400"
```

or Microsoft JSON date format:

```json
"/Date(1642625411380-0500)/"
```

---

## 3. Authorization and Request Signing

Secure functions require an `Authorization` header:

```http
Authorization: PWS {PAPIAccessID}:{Signature}
```

Rules:

- `PWS` must be uppercase.
- Do not include spaces before or after `:`.
- `{PAPIAccessID}` is assigned by Polaris.
- The private access key is assigned by Polaris and used to compute the HMAC signature.
- Hash algorithm is normally `HMACSHA1`, unless the server is configured for `HMACSHA256`.

### Signature Input

Build the signature over this concatenated string:

```text
{HTTPMethod}{URI}{HTTPDate}{PatronPasswordOrAccessSecret}
```

For non-patron-specific calls, omit the final password/access-secret portion:

```text
{HTTPMethod}{URI}{HTTPDate}
```

Important URI rule: sign the URI as the server receives it, not over-encoded.

Use:

```text
search/bibs/keyword/au?q=roald+dahl&sort=PDTI&limit=TOM=dvd
```

Not:

```text
search/bibs/keyword/au?q=roald%20dahl&sort=PDTI&limit=TOM%3Ddvd
```

### HMAC Pseudocode

```pseudo
data = HTTP_METHOD + URI + HTTP_DATE
if patron_password_or_access_secret is present:
    data += patron_password_or_access_secret

signature = Base64(HMAC_SHA1(data, private_access_key))
authorization = "PWS " + access_id + ":" + signature
```

### Authenticated Patron Flow

Use `AuthenticatePatron` before patron-specific methods. The response includes `AccessSecret`, which should be used instead of the patron password for later patron-method signatures.

`AccessSecret` typically expires 24 hours after authentication when non-legacy patron authentication is enabled.

For public methods called as an authenticated staff user, pass:

```http
X-PAPI-AccessToken: {AccessToken}
```

and use the `AccessSecret` when building the signature.

---

## 4. Response Conventions

PAPI frequently returns HTTP `200 OK` even when the operation failed at the application level. Always inspect:

```xml
<PAPIErrorCode>...</PAPIErrorCode>
<ErrorMessage>...</ErrorMessage>
```

General error-code conventions:

| Code | Meaning |
|---:|---|
| `0` | Success |
| Positive number | Usually count of rows returned or affected |
| `-1` | General failure |
| `-2` | Multiple errors; inspect returned rowset |
| `-3` | Partial failure; some items succeeded |
| `-5` | Database error |
| `-6` | Invalid parameter |
| `-9` | SQL timeout |
| `-11` | Validation errors |

Common families:

| Range | Meaning |
|---:|---|
| `-1000` to `-1499` | Bibliographic errors |
| `-1500` to `-1999` | MARC errors |
| `-2000` to `-2999` | Item / holdings errors |
| `-3000` to `-3999` | Patron errors |
| `-4000` to `-4999` | Hold request errors |
| `-5000` to `-5999` | Organization errors |
| `-6000` to `-6299` | Checkout / circulation errors |
| `-6300` to `-6999` | Checkin errors |
| `-8000` to `-8999` | Polaris user errors |
| `-9000` to `-9999` | Workstation errors |
| `-11000` to `-11999` | Record set errors |
| `-12000` to `-12999` | Acquisitions errors |

---

## 5. Common IDs and Defaults

### Language IDs

| ID | Language |
|---:|---|
| `1033` | English |
| `1042` | Korean |
| `1049` | Russian |
| `1065` | Farsi |
| `1066` | Vietnamese |
| `1141` | Hawaiian |
| `2052` | Chinese |
| `3082` | Spanish |
| `3084` | French |
| `12289` | Arabic |
| `15372` | Haitian Creole |

### Common Material Format IDs

| ID | Search code | Format |
|---:|---|---|
| `1` | `bks` | Book |
| `5` | `rec` | Sound Recording |
| `8` | `ser` | Serial |
| `24` | `per` | Periodical |
| `25` | `new` | Newspaper |
| `27` | `lpt` | Large Print |
| `33` | `dvd` | DVD |
| `35` | `mcd` | Music CD |
| `36` | `ebk` | eBook |
| `37` | `abk` | Audio Book |
| `40` | `brd` | Blu-ray Disc |
| `41` | `aeb` | eAudiobook |
| `44` | `vgm` | Video Game |
| `48` | `stm` | Streaming Music |
| `49` | `stv` | Streaming Video |
| `50` | `emg` | eMagazine |
| `51` | `vyl` | Vinyl |
| `52` | `abc` | Audio Book on CD |
| `53` | `abt` | Audio Book on Cassette |

---

## 6. Endpoint Reference

### 6.1 API Health / Version

#### Get API version

```http
GET /public/v1/{langID}/{appID}/{orgID}/api
```

Authorization: No.

Example:

```text
/public/v1/1033/100/1/api
```

Success includes version fields such as `Version`, `Major`, `Minor`, `Build`, and `Revision`.

---

## 7. Patron Authentication

### Authenticate patron

```http
POST /public/v1/{langID}/{appID}/{orgID}/authenticator/patron
```

Authorization: Yes.

Use HTTPS because the request body includes patron credentials.

XML body:

```xml
<PatronAuthenticationData>
  <Barcode>21756003332060</Barcode>
  <Password>1234</Password>
</PatronAuthenticationData>
```

Returns:

| Field | Meaning |
|---|---|
| `AccessToken` | Deprecated but still returned |
| `AccessSecret` | Use for future patron-method signing |
| `AuthExpDate` | Expiration, usually 24 hours when non-legacy auth is enabled |
| `PatronID` | Internal patron ID |

Common error:

| Code | Meaning |
|---:|---|
| `-3001` | Unable to authenticate patron credentials |

Security note: Three failed authentication attempts in five minutes may temporarily lock the account.

---

## 8. Bibliographic and Discovery Methods

### Get bib by BibID

```http
GET /public/v1/{langID}/{appID}/{orgID}/bib/{bibID}
```

Authorization: Yes if authentication level is `ALL`.

Example:

```text
/public/v1/1033/100/1/bib/5000
```

Returns bibliographic display rows such as:

| ElementID | Meaning |
|---:|---|
| `2` | Publisher |
| `3` | Description |
| `5` | Edition |
| `6` | ISBN |
| `7` | System Items Total |
| `8` | Current Holds |
| `9` | Summary |
| `13` | Call Number |
| `16` | System Items Available |
| `17` | Format |
| `18` | Author |
| `20` | Subject |
| `28` | Notes |
| `35` | Title |
| `48` | UPC |

Failure example: `PAPIErrorCode = -1`, `ErrorMessage = Invalid BibID`.

### Get bib by alternate key

```http
GET /public/v2/{langID}/{appID}/{orgID}/bib/{key}?type=barcode
```

Authorization: Yes if authentication level is `ALL`.

Example:

```text
/public/v2/1033/100/1/bib/5000?type=barcode
```

Common error:

| Code | Meaning |
|---:|---|
| `-6` | Invalid type supplied |

### Boolean bib search

```http
GET /public/v1/{langID}/{appID}/{orgID}/search/bibs/boolean?q={ccl}&sortby={sortby}
```

Authorization: Yes if authentication level is `ALL`.

Examples:

```text
/public/v1/1033/100/1/search/bibs/boolean?q=frog+sortby+PD/sort.descending
/public/v1/1033/100/1/search/bibs/boolean?q=AU%3dDahl%20AND%20TI%3dCharlie
```

Use Boolean search to retrieve titles attached to a heading ID:

```text
MAH={AuthorHeadingID}
MSH={SubjectHeadingID}
MTE={TitleEntryID}
MSE={SeriesEntryID}
```

Note: `HeadingsSearch` returns a hex `HeadingID`; convert it to decimal before using it in Boolean search.

Useful query parameters:

| Parameter | Meaning |
|---|---|
| `q` | CCL query, URL encoded |
| `sortby` | Sort order |
| `bibsperpage` | Page size, default `10` |
| `page` | Page number, default `1` |
| `limit` | Partial CCL filter, such as `TOM=dvd` or `TOM=dvd AND AB=74` |
| `notran` | Do not record search transaction |

Common `sortby` values:

```text
RELEVANCE
AU
TI
CALL
PD/sort.descending
PDTI
TIAU
TI_PD
TI_TOM
```

### Keyword bib search

```http
GET /public/v1/{langID}/{appID}/{orgID}/search/bibs/keyword/{qualifierName}?q={terms}
```

Authorization: Yes if authentication level is `ALL`.

Qualifiers:

```text
KW    keyword
TI    title
AU    author
SU    subject
NOTE  notes
PUB   publisher
GENRE genre
SE    series
ISBN  ISBN
ISSN  ISSN
LCCN  LCCN
PN    publisher number
LC    LC call number
DD    Dewey
LOCAL local control number
SUDOC SuDoc
CODEN CODEN
STRN  STRN
CN    control number
BC    barcode
```

Example:

```text
/public/v1/1033/100/3/search/bibs/keyword/au?q=roald%20dahl&sortby=PDTI&limit=TOM%3Ddvd&bibsperpage=2
```

Deprecated form:

```text
/public/v1/{langID}/{appID}/{orgID}/search/bibs/keyword/{qualifierName}/{terms}
```

### Get bib holdings

```http
GET /public/v1/{langID}/{appID}/{orgID}/bib/{bibID}/holdings
```

Authorization: Yes if authentication level is `ALL`.

Example:

```text
/public/v1/1033/100/1/bib/5000/holdings
```

Returns holdings rows with fields such as:

| Field | Meaning |
|---|---|
| `LocationID`, `LocationName` | Owning / assigned location |
| `CollectionID`, `CollectionName` | Collection |
| `Barcode` | Item barcode, when linked to item record |
| `CallNumber` | Local call number |
| `ShelfLocation` | Shelf location |
| `CircStatus` | Circulation status |
| `MaterialType` | Material type |
| `ItemsTotal`, `ItemsIn` | Availability counts |
| `Holdable` | Whether a hold can be placed |
| `VolumeNumber` | Volume number |
| `DueDate` | Due date, if applicable |

### Search headings

```http
GET /public/v1/{langID}/{appID}/{orgID}/search/headings/{qualifierName}
```

Authorization: Yes if authentication level is `ALL`.

Qualifiers:

```text
AU  Author
SE  Series
SU  Subject
TI  Title
```

Query parameters:

| Parameter | Required | Meaning |
|---|---:|---|
| `startpoint` | No | Starting term |
| `numterms` | Yes | Number of terms to return |
| `preferredpos` | Yes | Preferred response position of starting term |
| `notran` | No | Do not record search transaction |

Example:

```text
/public/v1/1033/100/1/search/headings/SU?startpoint=civil%20war&numterms=10&preferredpos=5
```

---

## 9. Lookup / Configuration Methods

### Get collections

```http
GET /public/v1/{langID}/{appID}/{orgID}/collections
```

Authorization: Yes.

Use `orgID = 1` for all system collections, or a branch org ID for branch-specific collections.

Returns:

| Field | Meaning |
|---|---|
| `ID` | Collection ID |
| `Name` | Display name |
| `Abbreviation` | Collection abbreviation |

### Get dates closed

```http
GET /public/v1/{langID}/{appID}/{orgID}/datesclosed
```

Authorization: Yes.

Returns `DateClosed` rows.

Common error:

| Code | Meaning |
|---:|---|
| `-5000` | Invalid OrganizationID supplied |

### Get item statuses

```http
GET /public/v1/{langID}/{appID}/{orgID}/itemstatuses
```

Authorization: Yes if authentication level is `ALL`.

Returns:

| Field | Meaning |
|---|---|
| `ItemStatusID` | Unique circulation status code |
| `Description` | Text description |
| `Name` | Status name used in circulation processing |
| `BannerText` | Web client display text |

---

## 10. Hold Request Methods

### Cancel one hold request

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/holdrequests/{requestID}/cancelled?wsid={workstationID}&userid={userID}
```

Authorization: Yes.

Cancelable statuses commonly include:

| Status | Meaning |
|---:|---|
| `1` | Inactive |
| `2` | Active |
| `4` | Pending |
| `7` | Not supplied |
| `18` | Located |
| `5` | Shipped, if enabled |
| `6` | Held, if enabled |

Example:

```text
/public/v1/1033/100/1/patron/21756003332022/holdrequests/803425/cancelled?wsid=1&userid=1
```

### Cancel all patron hold requests

Same endpoint as single cancel, but pass request ID `0`:

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/holdrequests/0/cancelled?wsid={workstationID}&userid={userID}
```

### Create hold request

```http
POST /public/v1/{langID}/{appID}/{orgID}/holdrequest
```

Authorization: Yes.

XML body:

```xml
<HoldRequestCreateData>
  <PatronID>299377</PatronID>
  <BibID>15592</BibID>
  <ItemBarcode/>
  <VolumeNumber/>
  <Designation/>
  <PickupOrgID>74</PickupOrgID>
  <IsBorrowByMail>0</IsBorrowByMail>
  <PatronNotes/>
  <ActivationDate>2009-11-17T09:28:00.00</ActivationDate>
  <WorkstationID>1</WorkstationID>
  <UserID>1</UserID>
  <RequestingOrgID>74</RequestingOrgID>
  <TargetGUID/>
  <HoldPickupAreaID>0</HoldPickupAreaID>
</HoldRequestCreateData>
```

Required fields:

| Field | Meaning |
|---|---|
| `PatronID` | Patron placing request |
| `BibID` | Bibliographic record |
| `PickupOrgID` | Pickup branch, or `0` for registered library |
| `WorkstationID` | Creating workstation |
| `UserID` | Polaris user |
| `RequestingOrgID` | Requesting branch |

Returned workflow fields:

| Field | Meaning |
|---|---|
| `RequestGUID` | Conversation ID for multi-step hold process |
| `TxnGroupQualifier` | Transaction group value |
| `TxnQualifier` | Transaction value |
| `StatusType` | `1` error, `2` answer/final, `3` conditional |
| `StatusValue` | Specific result or conditional |
| `Message` | User-facing message |
| `QueuePosition` | Queue position if placed |
| `QueueTotal` | Total queue size if placed |

Conditional `StatusValue` values:

| Value | Meaning |
|---:|---|
| `3` | Item available locally |
| `4` | Accept ILL policy |
| `5` | Existing holds warning |
| `6` | No items linked |
| `7` | Accept local hold charge policy |

### Reply to hold request conditional

```http
PUT /public/v1/{langID}/{appID}/{orgID}/holdrequest/{requestGUID}
```

Authorization: Yes.

XML body order matters:

```xml
<HoldRequestReplyData>
  <TxnGroupQualifier>BQeK9WLrNew_0BRKISzr2G</TxnGroupQualifier>
  <TxnQualifier>496QYG6MMKss1C6nagTGkG</TxnQualifier>
  <RequestingOrgID>74</RequestingOrgID>
  <Answer>1</Answer>
  <State>3</State>
</HoldRequestReplyData>
```

`Answer` values:

| Value | Meaning |
|---:|---|
| `1` | Yes |
| `0` | No / cancel |

`State` values:

| Value | Meaning |
|---:|---|
| `1` | Item available locally |
| `2` | Accept ILL policy |
| `3` | Accept even with existing holds |
| `4` | No items attached, still place hold |
| `5` | Accept local hold policy / charge |

### Change hold pickup branch

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/holdrequests/{requestID}/pickupbranch?userid={userID}&wsid={workstationID}&pickupbranchid={pickupBranchID}
```

Authorization: Yes.

Invalid status example:

| Code | Meaning |
|---:|---|
| `-4016` | Cannot change pickup branch for request in current status |

### Suspend or reactivate one hold

Suspend:

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/holdrequests/{requestID}/inactive
```

Reactivate:

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/holdrequests/{requestID}/active
```

XML body order matters:

```xml
<HoldRequestActivationData>
  <UserID>1</UserID>
  <ActivationDate>2009-12-31T00:00:00.00</ActivationDate>
</HoldRequestActivationData>
```

### Suspend or reactivate all holds

Same endpoint, but pass request ID `0`:

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/holdrequests/0/inactive
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/holdrequests/0/active
```

---

## 11. ILL Request Methods

### Cancel one or all ILL requests

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/illrequests/{illRequestID}/cancelled?wsid={workstationID}&userid={userID}
```

Pass `illRequestID = 0` to cancel all eligible ILL requests for the patron.

### Create ILL request

```http
POST /public/v1/{langID}/{appID}/{orgID}/illrequest
```

Authorization: Yes.

XML body:

```xml
<ILLRequestCreateData xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
  <PatronID>67794</PatronID>
  <Title>Black pioneers in American history</Title>
  <ISBN/>
  <LCCN/>
  <ItemType>0</ItemType>
  <MediumType>1</MediumType>
  <PickupOrgID>99</PickupOrgID>
  <WorkstationID>1243</WorkstationID>
  <UserID>1</UserID>
  <HoldPickupAreaID>1</HoldPickupAreaID>
</ILLRequestCreateData>
```

Required fields:

| Field | Meaning |
|---|---|
| `PatronID` | Patron placing request |
| `Title` | Primary title |
| `PickupOrgID` | Pickup branch, or `0` for registered library |

Optional bibliographic fields include `Author`, `Publisher`, `Edition`, `PublicationDate`, `ISBN`, `ISSN`, and `LCCN`.

---

## 12. Circulation Methods

### Checkout item to patron

```http
POST /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/itemsout
```

Authorization: Yes.

This endpoint automatically tries to renew the item if the specified patron already has the item checked out, unless renewals are blocked for the transacting branch.

XML body:

```xml
<ItemCheckoutData>
  <ItemBarcode>0000410443451</ItemBarcode>
  <LogonBranchID>99</LogonBranchID>
  <LogonUserID>1</LogonUserID>
  <LogonWorkstationID>1243</LogonWorkstationID>
</ItemCheckoutData>
```

Returns fields such as:

| Field | Meaning |
|---|---|
| `ItemRecordID` | Internal item record ID |
| `IsRenewal` | Whether checkout became renewal |
| `DueDate` | Due date |
| `ChargeAmount` | Checkout charge, if any |
| `PatronBlockFlags` | Patron block bitmask |
| `ItemBlockFlags` | Item block bitmask |
| `RenewalBlockFlags` | Renewal block bitmask |
| `MaterialTypeID` | Material type |
| `Title` | Item title |

Important errors:

| Code | Meaning |
|---:|---|
| `-6101` | Patron blocked |
| `-6112` | Item blocked |
| `-6119` | Renewal blocked |

### Renew one item

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/itemsout/{itemRecordID}
```

Authorization: Yes.

XML body order matters:

```xml
<ItemsOutActionData>
  <Action>renew</Action>
  <LogonBranchID>74</LogonBranchID>
  <LogonUserID>1</LogonUserID>
  <LogonWorkstationID>1</LogonWorkstationID>
  <RenewData>
    <IgnoreOverrideErrors>true</IgnoreOverrideErrors>
  </RenewData>
</ItemsOutActionData>
```

Successful renewals include `DueDateRows`. Blocks include `BlockRows`.

### Renew all items for patron

Same endpoint, but pass item ID `0`:

```http
PUT /public/v1/{langID}/{appID}/{orgID}/patron/{patronBarcode}/itemsout/0
```

---

## 13. Protected Methods Inventory

The uploaded reference also lists these protected methods. Use the full vendor reference for the complete request/response bodies.

```text
AuthenticateStaffUser
BibsPost
HoldRequestGetList
ItemCheckinPost
ItemUpdateBarcode
JobsPurchaseOrdersPost
JobsPurchaseOrdersPut
JobsPurchaseOrdersResultGet
JobsPurchaseOrdersStatusGet
NotificationUpdate
PatronAccountPay
PatronAccountPayAll
PatronAccountVoid
PatronAccountCreateCredit
PatronAccountDepositCredit
PatronAccountRefundCredit
CreatePatronBlocks
PatronRenewBlocksGet
PatronSearch
RecordSetContentPut
RecordSetRecordsGet
RemoteStorageItemsGet
SAMobilePhoneCarriersGetResult
UpdatePatronNotesData
RequestsUpdateStatus
```

---

## 14. Synchronization Methods Inventory

The uploaded reference includes sync methods for discovery interfaces and circulation workflows. Use these for integration jobs that need deltas, IDs, or record exports.

### 3rd Party Discovery Interfaces

```text
Patron_GetBarcodeFromID
Synch_AuthsByIDGet
Synch_BibsByIDGet
Synch_BibsPagedGet
Synch_BibReplacementIDGet
Synch_GetAuthIDList
Synch_GetBibIDList
Synch_GetBibResourceCountsByID
Synch_GetDeletedAuths
Synch_GetDeletedAuthsPaged
Synch_GetDeletedBibs
Synch_GetDeletedBibsPaged
Synch_GetDeletedItems
Synch_GetDeletedItemsPaged
Synch_GetDeletedPatrons
Synch_GetDeletedPatronsPaged
Synch_GetItemIDList
Synch_GetMaxAuthID
Synch_GetMaxBibID
Synch_GetSerialCompressedHoldingsByID
Synch_GetSerialCompressedHoldingsPaged
Synch_GetMaxItemID
Synch_GetSubscriptionsByID
Synch_GetUpdatedAuths
Synch_GetUpdatedAuthsPaged
Synch_GetUpdatedBibs
Synch_GetUpdatedBibsPaged
Synch_GetUpdatedItems
Synch_GetUpdatedItemsPaged
Synch_ItemsByBibIDGet
Synch_ItemByIDGet
```

### 3rd Party Circulation

```text
SynchTasksCheckout
SynchTasksCheckin
SynchTasksExpireCopy
SynchTasksNotifyPatron
SynchTasksNotifyPatronItem
SynchTasksPullMARCData
```

---

## 15. Implementation Checklist for Coding Agents

When building or modifying a PAPI client:

1. Always construct the route from `{scope}/{version}/{langID}/{appID}/{orgID}`.
2. Use HTTPS unless the environment explicitly allows HTTP for public methods.
3. Add `Accept` and `Content-Type` headers explicitly.
4. Generate `Date` or `PolarisDate` in RFC1123 GMT format.
5. Build the HMAC signature from method + server-received URI + date + optional patron secret.
6. Use `AccessSecret`, not patron password, after `AuthenticatePatron`.
7. Do not rely on HTTP status alone; inspect `PAPIErrorCode`.
8. Treat positive `PAPIErrorCode` values as success row counts unless endpoint docs say otherwise.
9. Preserve XML element order where the docs say order matters.
10. For bulk operations, pass ID `0` where documented:
    - cancel all holds
    - suspend/reactivate all holds
    - cancel all ILL requests
    - renew all items
11. For multi-step hold requests, continue calling `HoldRequestReply` until `StatusType` is `1` or `2`.
12. URL encode query-string values, but sign the URI in the normalized form expected by PAPI.
13. Log request method, normalized URI, PAPI error code, and error message, but never log patron passwords or access secrets.

---

## 16. Minimal Client Flow Examples

### Search catalog by author

```text
GET /public/v1/1033/100/1/search/bibs/keyword/AU?q=roald%20dahl&sortby=PDTI&bibsperpage=10
```

Then inspect:

```text
BibSearchRows[*].ControlNumber
BibSearchRows[*].Title
BibSearchRows[*].Author
BibSearchRows[*].SystemItemsIn
BibSearchRows[*].SystemItemsTotal
```

### Authenticate patron, then renew all items

1. Authenticate:

```text
POST /public/v1/1033/100/1/authenticator/patron
```

2. Save `AccessSecret`.

3. Sign the renewal request with `AccessSecret`.

4. Renew all:

```text
PUT /public/v1/1033/100/1/patron/{patronBarcode}/itemsout/0
```

Body:

```xml
<ItemsOutActionData>
  <Action>renew</Action>
  <LogonBranchID>1</LogonBranchID>
  <LogonUserID>1</LogonUserID>
  <LogonWorkstationID>1</LogonWorkstationID>
  <RenewData>
    <IgnoreOverrideErrors>true</IgnoreOverrideErrors>
  </RenewData>
</ItemsOutActionData>
```

### Place hold with conditional handling

1. Create hold:

```text
POST /public/v1/1033/100/1/holdrequest
```

2. If response has:

```text
StatusType = 3
```

then call:

```text
PUT /public/v1/1033/100/1/holdrequest/{RequestGUID}
```

with the returned `TxnGroupQualifier`, `TxnQualifier`, and appropriate `Answer` / `State`.

3. Stop when:

```text
StatusType = 1  # error
StatusType = 2  # final answer
```

---

## 17. Notes for Agent Use

- Prefer JSON for new client code unless existing systems require XML.
- Still be prepared to handle XML examples and XML-only documentation.
- Treat PAPI method names as domain operations, not pure REST resources; many updates are `PUT` actions against action-like paths.
- Several “public” endpoints still require PAPI authorization depending on `AuthenticationLevel`.
- Some local behavior depends on Polaris System Administration configuration, so successful signatures do not guarantee business-rule success.

# Polaris API C# Client Integration Todo List

This file tracks the implementation progress of missing API endpoints, models, and interface declarations based on the comparison with the `compiled_html_documentation.md`.

---

## 1. Interface Fixes (Missing from `IPapiClient`)
Expose already-implemented helper methods in the `IPapiClient` interface.

- [ ] Add `BibBooleanSearch` to `IPapiClient` interface
- [ ] Add `BibKeywordSearch` to `IPapiClient` interface

---

## 2. Core Services & Circulation Endpoints
Implement core circulation and patron-facing endpoints.

### ILL (Interlibrary Loan)
- [ ] Implement **`ILLRequestPost`** (`POST /public/v1/{LangID}/{AppID}/{OrgID}/illrequest`)
- [ ] Implement **`ILLRequestCancelPut`** (`PUT /public/v1/{LangID}/{AppID}/{OrgID}/patron/{PatronBarcode}/illrequests/{illRequestID}/cancelled`)
- [ ] Add dedicated **`HoldRequestSuspendAllForPatron`** / **`HoldRequestReactivateAllForPatron`** wrapper helpers (calling suspend/reactivate with Request ID `0`)

### Item Checkout / Check-in
- [ ] Implement **`ItemCheckinPost`** (`POST /protected/v1/{LangID}/{AppID}/{OrgID}/item/{ItemBarcode}/checkin`)
- [ ] Implement **`ItemCheckoutPost`** (`POST /public/v1/{LangID}/{AppID}/{OrgID}/patron/{PatronBarcode}/itemsout`)

### Bibliographic Utilities & Areas
- [ ] Implement **`BibsPost`** (`POST /protected/v1/{LangID}/{AppID}/{OrgID}/{accesstoken}/bibs`) — MARC import/overlay
- [ ] Implement **`MultipartGet`** (`GET /public/v1/bib/{BibID}/multiparts`)
- [ ] Implement **`PickupAreasGet`** (`GET /public/v1/{LangID}/{AppID}/{OrgID}/pickupareas`)
- [ ] Implement **`SAMobilePhoneCarriersGetResult`** (`GET /protected/v1/{LangID}/{AppID}/{OrgID}/{AccessToken}/sysadmin/mobilephonecarriers`)
- [ ] Implement **`RequestsUpdateStatus`** (`PUT /protected/v1/{LangID}/{AppID}/{OrgID}/{access_token}/circulation/requests/{request_id}/status?action={status_action}`)

---

## 3. Versioning & Advanced Patron Updates
Add newer V2 routes where the client is outdated.

- [ ] Implement **`BibGetByType Version 2`** (`GET /public/v2/{LangID}/{AppID}/{OrgID}/bib/{key}?type=barcode`)
- [ ] Implement **`PatronRegistrationUpdate Version 2`** (`PUT /public/v2/{LangID}/{AppID}/{OrgID}/patron/{PatronBarcode}`)

---

## 4. Jobs & Purchase Orders
Implement support for PO and background job status tracking.

- [ ] Implement **`JobsPurchaseOrdersPost`** (`POST /protected/v1/.../jobs/purchaseorders`)
- [ ] Implement **`JobsPurchaseOrdersPut`** (`PUT /protected/v1/.../jobs/purchaseorders/{JobID}`)
- [ ] Implement **`JobsPurchaseOrdersResultGet`** (`GET /protected/v1/.../jobs/purchaseorders/{JobID}/results`)
- [ ] Implement **`JobsPurchaseOrdersStatusGet`** (`GET /protected/v1/.../jobs/purchaseorders/{JobID}/status`)

---

## 5. Synch Deleted & Updated Endpoints (Discovery / DB Sync)
Implement all endpoints used for keeping external search indexes in sync with Polaris.

### Authority Synch
- [ ] Implement `Synch_GetDeletedAuths`
- [ ] Implement `Synch_GetDeletedAuthsPaged`
- [ ] Implement `Synch_GetUpdatedAuths`
- [ ] Implement `Synch_GetUpdatedAuthsPaged`

### Bibliographic Synch
- [ ] Implement `Synch_GetDeletedBibs`
- [ ] Implement `Synch_GetDeletedBibsPaged`
- [ ] Implement `Synch_GetUpdatedBibs`
- [ ] Implement `Synch_GetUpdatedBibsPaged`

### Item Synch
- [ ] Implement `Synch_GetDeletedItems`
- [ ] Implement `Synch_GetDeletedItemsPaged`
- [ ] Implement `Synch_GetUpdatedItems`
- [ ] Implement `Synch_GetUpdatedItemsPaged`

### Patron Synch
- [ ] Implement `Synch_GetDeletedPatrons`
- [ ] Implement `Synch_GetDeletedPatronsPaged`

---

## 6. Required Data Models
Create matching request/response models in the `Models/` directory.

### ILL Models
- [ ] `ILLRequestCreateData` (Body)
- [ ] `ILLRequestResult` (Response)
- [ ] `ILLRequestCancelResult`
- [ ] `ILLRequestCancelRows` / `ILLRequestCancelRow`

### Circulation & Checkout Models
- [ ] `ItemCheckInData` (Body)
- [ ] `ItemCheckInResult` (Response)
- [ ] `ItemCheckoutData` (Body)
- [ ] `ItemCheckoutResult` (Response)

### Utilities Models
- [ ] `BibsPostResult` (for `BibsPost`)
- [ ] `PickupAreasGetResult` (for `PickupAreasGet`)
- [ ] `SAMobilePhoneCarriersGetResult` (for phone carriers)
- [ ] Jobs and Purchase Order request/response classes

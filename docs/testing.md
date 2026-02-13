---
layout: default
title: Testing
nav_order: 13
---


# Testing

The project includes data provider tests. Full API conformance testing can be performed using the [OGC API - Features Conformance Test Suite](https://opengeospatial.github.io/ets-ogcapi-features10/).

## Test Application

The test application is included in the repository (`SampleWebApplication`). To run the OGC API conformance tests, you need to launch the Aspire application with the `tests` profile. This profile initializes the following components:

- **SQL Server**: Used as the database
- **PostgreSQL (PostGIS)**: Database with spatial extensions
- **OGC Tests Container**: Based on the [ogccite/ets-ogcapi-features10](https://hub.docker.com/r/ogccite/ets-ogcapi-features10) Docker image, which includes the TEAM Engine application and the OGC API - Features test suite

### Important Notes
- Clean up database volumes when switching launch profiles to avoid conflicts
- Manually delete volumes if persistent database issues occur
- Test collections are defined in `ogcapi-tests.json` (located alongside `SampleWebApplication`)
- **Critical requirement**: The OGC API - Features 1.0 test suite requires OpenAPI 3.0 specification

## Running OGC Tests

To run the OGC API conformance tests:
1. Open the TEAM Engine application (URL available in Aspire dashboard for `ogc-tests` resource)
2. Log in with credentials:  
   **Username**: `ogctest`  
   **Password**: `ogctest`
3. Click `Create a new session`
4. In the "Specification" dropdown, select **OGC API - Features** and click `Start new session`
5. In *Location of the landing page*, enter your API URL (default):  
   ```
   http://host.docker.internal:5000/api/ogc
   ```  
   *Tip: Find the exact URL in `ogc-tests` resource environment variables under `services__web-application__http__0`*
6. (Optional) Specify collections to test or select all
7. Click **Start** to begin testing

## Additional Information

- Test configuration is automatically loaded from `ogcapi-tests.json`
- For more details about the OGC API - Features conformance tests, refer to the [TEAM Engine Documentation](https://opengeospatial.github.io/ets-ogcapi-features10/)
- The Docker image used for testing is available on Docker Hub: [ogccite/ets-ogcapi-features10](https://hub.docker.com/r/ogccite/ets-ogcapi-features10)
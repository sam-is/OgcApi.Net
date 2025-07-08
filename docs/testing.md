---
layout: default
title: Testing
nav_order: 13
---


# Testing

Currently, this project contains tests for data providers. Testing the entire API can be done using the [OGC API - Features Conformance Test Suite](https://cite.opengeospatial.org/te2/about/ogcapi-features-1.0/1.0/site/).

## Test Application

The test application is included in the repository (`SampleWebApplication`). To run the OGC API conformance tests, you need to launch the Aspire application with the `tests` profile. This profile initializes the following components:

- **SQL Server**: Used as the database.
- **PostgreSQL (PostGIS)**: Another database with PostGIS support.
- **OGC Tests Container**: A container based on the [ogccite/ets-ogcapi-features10](https://hub.docker.com/r/ogccite/ets-ogcapi-features10) Docker image, which includes the TEAM Engine application and the OGC API - Features test suite.

### Important Notes
- When switching between different launch profiles, make sure to clean up any existing database volumes to avoid conflicts.
- In case of persistent database issues, delete the associated volumes manually.
- The collections used for testing are described in the `ogcapi-tests.json` file, located alongside the `SampleWebApplication`.

## Launching the Test Application

To run the test application:
Start the Aspire project with the `tests` profile.

For more information about TEAM Engine, refer to the official documentation: [TEAM Engine Documentation](https://cite.opengeospatial.org/teamengine/about/ogcapi-features-1.0/1.0/site/).

## Running OGC Tests

To run the OGC API conformance tests:
1. Open the TEAM Engine application at url of ogc-tests resource.
2. Create an account using the "Create an account" link. Note that accounts are reset when the Aspire application is restarted, so you will need to recreate the account after each restart.
3. Log in with the created account.
4. Click "Create a new session"
5. In the "Specification" dropdown, select **OGC API - Features** and click "Start new session"
6. In the "Location of the landing page" field, enter the URL of the server's landing page. By default, this is:
   ```
   http://host.docker.internal:5000/api/ogc
   ```
   You can also find the exact URL in the environment variables of the `ogc-tests` resource under the key `services__web-application__http__0`.
7. Optionally, specify a fixed number of collections to test or select all collections.
8. Click "Start" to begin the tests.

## Additional Information

- The test configuration is defined in the `ogcapi-tests.json` file, which is automatically loaded by the test application.
- For more details about the OGC API - Features conformance tests, refer to the [TEAM Engine Documentation](https://cite.opengeospatial.org/teamengine/about/ogcapi-features-1.0/1.0/site/).
- The Docker image used for testing is available on Docker Hub: [ogccite/ets-ogcapi-features10](https://hub.docker.com/r/ogccite/ets-ogcapi-features10).
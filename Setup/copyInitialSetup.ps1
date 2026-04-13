Copy-Item -Path "Setup/InitialSetup/WebResources" -Destination "src/Dataverse" -Recurse -Force
Copy-Item -Path "Setup/InitialSetup/SharedTest" -Destination "test" -Recurse -Force
Copy-Item -Path "Setup/InitialSetup/SharedContext" -Destination "src/Shared" -Recurse -Force

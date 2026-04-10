Copy-Item -Path "DemoSetup/InitialSetup/WebResources" -Destination "src/Dataverse" -Recurse -Force
Copy-Item -Path "DemoSetup/InitialSetup/SharedTest" -Destination "test" -Recurse -Force
Copy-Item -Path "DemoSetup/InitialSetup/SharedContext" -Destination "src/Shared" -Recurse -Force

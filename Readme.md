HostingEnvironment instantiates PhysicalFileProvider which is later used by components like Razor Pages to watch for file changes.
This app reproduces the issue where we do not dispose the HostingEnvironment instance on TestServer shutdown causing memory to increase rapidly.
HostingEnviroment instance does not currently implement IDisposable.

Scenario:
=========
User has a web application and some functional tests for it. He makes a change to razor pages file, saves it and runs the tests again.

Repro steps:
===========
1. Open the FileSystemWatcherRepro.sln
2. Build the solution and run the tests in WebApp.Test. This time tests should run quickly.
3. Make a change(like edit the Title html tag) in a razor page under WebApp\Pages\Foo.cshtml and save the file.
4. Run the tests again and now you should see(ex: TaskManager) the memory increase rapidly.


Current thoughts on why this is happening:
=========================================
Each new instance of PhysicalFileProvider creates a new instnace of FileSystemWatcher (on calling Watch on the fileprovider). This appears
to create pinned handles by the FileSystemWatcher. Since we run like, 100 tests, this creates lot of pinned handles and since pinned handles
cause heap fragmentation, the size of memory keeps increasing.


Testing Workaround:
===================
Uncomment the code in Dispose method of the test. This would dispose the file providers hanging off of HostingEnvironment and now the tests
will run fine.
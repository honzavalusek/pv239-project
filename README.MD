# Maly Farmar
## Running the App
1. (If `appsettings.Development.json` doesn't already exist,) Rename `appsettings.Development.json.example` to `appsettings.Development.json` 
   and fill the Api BaseUrl. Use `http://localhost:5138/` if you want to run the app on simulator
   or `http://<<your_device_ip>>:5138/` if you want to run the app on a real device connected to
   the same network as your computer.
2. Run API migrations (classic entity migrations) and seeders (run the migrations project).
3. Run API.
4. Run the app on your simulator or real device.


# MalyFarmar.Api.DAL

## Prerequisites

Before proceeding, ensure you have the following tools installed:

1. **Spatialite Tools**  
   Install using Homebrew:  
   ```bash
   brew install spatialite-tools
   ```

2. **Libspatialite**  
   Install using Homebrew:  
   ```bash
   brew install libspatialite
   ```

## Running Migrations

Before running migrations or seeders, set the `DYLD_LIBRARY_PATH` environment variable:

```bash
export DYLD_LIBRARY_PATH=/opt/homebrew/lib:$DYLD_LIBRARY_PATH
```

This ensures the required libraries are correctly linked.

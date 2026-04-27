#include <windows.h>
#include <stdio.h>
#include <vector>
#include <windows.h>
#include <pdh.h>
#pragma comment(lib, "pdh.lib")

void GetCpuCacheInfo() {
  DWORD returnLength = 0;
  // Call once to get required buffer size
  GetLogicalProcessorInformation(NULL, &returnLength);

  std::vector<SYSTEM_LOGICAL_PROCESSOR_INFORMATION> buffer(returnLength / sizeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
  if(GetLogicalProcessorInformation(&buffer[0], &returnLength)) {
    unsigned int level1 = 0, level2 = 0, level3 = 0;
    for(const auto& info : buffer) {
      if(info.Relationship == RelationCache) {
        CACHE_DESCRIPTOR cache = info.Cache;
        switch(cache.Level) {
          case 1: 
            level1 += cache.Size / 1024; 
            break;
          case 2: 
            level2 += cache.Size / 1024;
            break;
          case 3: 
            level3 += cache.Size / 1024;
            break;
          default:
            break;
        }
        printf("L%d Cache: %d KB, Line Size: %d bytes\n",
          cache.Level, cache.Size / 1024, cache.LineSize);
      }
    }
    printf("+Total L1 Cache: %d KB\n", level1);
    printf("+Total L2 Cache: %d KB\n", level2);
    printf("+Total L3 Cache: %d KB\n", level3);
  }
}

double GetTotalCPULoad() {
  PDH_HQUERY cpuQuery;
  PDH_HCOUNTER cpuTotal;
  PdhOpenQuery(NULL, NULL, &cpuQuery);
  // Use English counter to avoid localization issues
  PdhAddEnglishCounter(cpuQuery, L"\\Processor(_Total)\\% Processor Time", NULL, &cpuTotal);
  PdhCollectQueryData(cpuQuery);

  // CPU load is a measurement over an interval
  Sleep(1000);

  PDH_FMT_COUNTERVALUE counterVal;
  PdhCollectQueryData(cpuQuery);
  PdhGetFormattedCounterValue(cpuTotal, PDH_FMT_DOUBLE, NULL, &counterVal);
  PdhCloseQuery(cpuQuery);

  return counterVal.doubleValue;
}

int main() {
  GetCpuCacheInfo();
  
  while(true) {
    double cpuLoad = GetTotalCPULoad();
    printf("Total CPU Load: %.2f%%\n", cpuLoad);
  }
  return 0;
}

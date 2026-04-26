#include <windows.h>
#include <stdio.h>
#include <vector>

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

int main() {
  GetCpuCacheInfo();
  return 0;
}

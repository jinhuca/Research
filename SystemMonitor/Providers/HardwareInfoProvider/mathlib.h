#pragma once
#include "CpuInstructionSet.h"
#include <oleauto.h>

extern "C" __declspec(dllexport) int AddNumbers(int a, int b) {
  return a + b;
}

extern "C" __declspec(dllexport) BSTR __stdcall GetVendor() {
  std::string vendor = InstructionSet::Vendor();
  return SysAllocString(std::wstring(vendor.begin(), vendor.end()).c_str());
}

extern "C" __declspec(dllexport) BSTR __stdcall GetBrand() {
  std::string brand = InstructionSet::Brand();
  return SysAllocString(std::wstring(brand.begin(), brand.end()).c_str());
}

extern "C" __declspec(dllexport) bool __stdcall Is_3DNOW() {
  return InstructionSet::_3DNOW();
}

extern "C" __declspec(dllexport) bool __stdcall Is_XSAVE() {
  return InstructionSet::XSAVE();
}

extern "C" {
  struct CpuInstructionSet {
    //const char* brand;
    bool is_3DNOW;
    bool is_3DNOWEXT;
    //bool is_XSAVE;
  };

  __declspec(dllexport) void GetData(CpuInstructionSet* data) {
    //data->brand = InstructionSet::Brand().c_str();
    data->is_3DNOW = InstructionSet::_3DNOW();
    data->is_3DNOWEXT = InstructionSet::_3DNOWEXT();
    //data->is_XSAVE = InstructionSet::XSAVE();
  }
}
#pragma once
#include "CpuInstructionSet.h"

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
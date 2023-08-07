#include <windows.h>
#include <iostream>
#include <psapi.h>
#include <string>
#include <format>
#include <TlHelp32.h>
#include <iostream>
#include <iostream>
#include <stdio.h>
#include <tchar.h>
#include <tlhelp32.h>
#include <codecvt>
#include "MemRead.hpp"


void hex(uintptr_t n) {
	printf("%X\n", n);
}
uintptr_t GetProcessIdByName(const std::wstring& processName) {
	uintptr_t processId = 0;

	HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	if (snapshot != INVALID_HANDLE_VALUE) {
		PROCESSENTRY32 processEntry = { sizeof(PROCESSENTRY32) };
		if (Process32First(snapshot, &processEntry)) {
			do {
				if (processName.compare(processEntry.szExeFile) == 0) {
					processId = processEntry.th32ProcessID;
					break;
				}
			} while (Process32Next(snapshot, &processEntry));
		}
		CloseHandle(snapshot);
	}

	std::wcout << L"Process ID of " << processName << L": " << processId << std::endl;

	return processId;
}
int main()
{
	auto gameName = (LPCWSTR)L"GH.exe";
	const wchar_t* monoDllName = L"mono-2.0-bdwgc.dll";
	DWORD procID = GetProcessIdByName(gameName);
	HANDLE procHandle = OpenProcess(
		PROCESS_QUERY_INFORMATION
		| PROCESS_VM_READ
		| PROCESS_VM_OPERATION
		| PROCESS_VM_WRITE,
		FALSE, procID);
	Mem::initProcess(procHandle);

	HMODULE handleModules[1024];
	DWORD cbNeeded;
	uintptr_t baseAddress = 0;
	if (EnumProcessModules(procHandle, handleModules, sizeof(handleModules), &cbNeeded)) {
		//get module at first index
	}
	if (cbNeeded / sizeof(HMODULE) > 0) {
		baseAddress = (uintptr_t)handleModules[0];
	}
	HMODULE monoModule = 0;
	uintptr_t monoBaseAddress = 0;
	MODULEINFO* monoModuleInfo;

	for (int i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
	{
		MODULEINFO moduleInfoBuffer;
		wchar_t szModName[MAX_PATH];
		auto moduleHandle = handleModules[i];
		GetModuleInformation(procHandle, moduleHandle,
			&moduleInfoBuffer,
			sizeof(MODULEINFO));

		auto moduleName = GetModuleBaseNameW(procHandle, moduleHandle,
			szModName, sizeof(szModName) / sizeof(wchar_t));

		//std::wcout << szModName << " < this mod name\n";

		if (std::wstring(szModName).compare(monoDllName) == 0) {
			monoModule = moduleHandle;
			monoModuleInfo = &moduleInfoBuffer;
			monoBaseAddress = reinterpret_cast<uintptr_t>(moduleInfoBuffer.lpBaseOfDll);
		}
	}


	typedef void* (*MonoGetRootDomainFunct)();

	std::cout << "Base Address: " << std::hex << baseAddress << std::endl;

	std::wcout << "mono-2.0-bwgc.dll base address " << std::hex << monoBaseAddress << '\n';

	// decode date time  23:13 7/8/2556
	// in Cheat Engine
	//mono-2.0-bdwgc.dll + A4C30 || 48 8B 05 A9A77100 || mov rax, [mono - 2.0 - bdwgc.dll + 7BF3E0] { (220AA112D20) }
	Mem::initProcess(procHandle);
	auto rootDomainAddr = Mem::readIntPtr(monoBaseAddress + 0x7BF3E0);
	std::wcout << "mono root domain: " << std::hex << rootDomainAddr << '\n';

	auto playerAddr = Mem::readIntPtr(rootDomainAddr + 0x5FD0);
	auto playerSpeedAddr = playerAddr + 0x484;
	std::wcout << "player address: " << std::hex << playerAddr << '\n';

	while (true) {
		Sleep(100);
		auto currentSpeed = Mem::readFloat(playerSpeedAddr);
		std::wcout << "read speed " << currentSpeed << "\n";
	}
	CloseHandle(procHandle);
	return 0;
}

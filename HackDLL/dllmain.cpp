// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <windows.h>
#include <iostream>
#include <psapi.h>
#include <string>
#include <format>
#include <TlHelp32.h>
#include <stdio.h>
#include <tchar.h>
#include <tlhelp32.h>
#include <codecvt>
#include "MemRead.hpp"
#include "../LearnHackMono/Mono.h"

uintptr_t getProcessBaseAddress(DWORD procID) {
	HANDLE procHandle = OpenProcess(
		PROCESS_QUERY_INFORMATION
		| PROCESS_VM_READ
		| PROCESS_VM_OPERATION
		| PROCESS_VM_WRITE,
		FALSE, procID);

	HMODULE handleModules[1024];
	DWORD cbNeeded;
	uintptr_t baseAddress = 0;
	if (EnumProcessModules(procHandle, handleModules, sizeof(handleModules), &cbNeeded)) {
		//get module at first index
		auto moduleHandle = handleModules[0];
		MODULEINFO moduleInfoBuffer;
		GetModuleInformation(procHandle, moduleHandle, &moduleInfoBuffer, sizeof(MODULEINFO));
		return uintptr_t(moduleInfoBuffer.lpBaseOfDll);
	}

	return 0;
}
uintptr_t getProcessIdByName(const std::wstring& processName) {
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

HMODULE m_hModule;
HANDLE myThreadHandle = 0;

void launchHackThread() {
	AllocConsole();

	freopen("CONOUT$", "w", stdout);
	std::cout << "========[Green Hell Hack DLL]========\n";
	std::cout << "Handle module " << std::hex << m_hModule << "\n";
	const wchar_t* monoDllName = L"mono-2.0-bdwgc.dll";
	DWORD gameProcID = GetCurrentProcessId();
	HANDLE gameProcHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, gameProcID);
	Mem::setTargetProcess(gameProcHandle);

	HMODULE handleModules[1024];
	DWORD cbNeeded;
	uintptr_t baseAddress = 0;
	EnumProcessModules(gameProcHandle, handleModules, sizeof(handleModules), &cbNeeded);
	if (cbNeeded / sizeof(HMODULE) > 0) {
		baseAddress = (uintptr_t)handleModules[0];
	}
	HMODULE monoModule = 0;
	uintptr_t monoBaseAddress = 0;
	MODULEINFO* monoModuleInfo;

	//get mono-2.0-bdwgc.dll
	for (int i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
	{
		MODULEINFO moduleInfoBuffer;
		wchar_t szModName[MAX_PATH];
		auto moduleHandle = handleModules[i];
		GetModuleInformation(gameProcHandle, moduleHandle,
			&moduleInfoBuffer,
			sizeof(MODULEINFO));

		auto moduleName = GetModuleBaseNameW(gameProcHandle, moduleHandle,
			szModName, sizeof(szModName) / sizeof(wchar_t));
		//std::wcout << szModName << " < this mod name\n";
		if (std::wstring(szModName).compare(monoDllName) == 0) {
			monoModule = moduleHandle;
			monoModuleInfo = &moduleInfoBuffer;
			monoBaseAddress = reinterpret_cast<uintptr_t>(moduleInfoBuffer.lpBaseOfDll);
		}
	}


	typedef void* (*MonoGetRootDomainFunct)();
	std::cout << "Game Process base address: " << std::hex << baseAddress << std::endl;
	std::wcout << "mono-2.0-bwgc.dll base address " << std::hex << monoBaseAddress << '\n';

	// decode date time  23:13 7/8/2556
	// in Cheat Engine
	//mono-2.0-bdwgc.dll + A4C30 || 48 8B 05 A9A77100 || mov rax, [mono - 2.0 - bdwgc.dll + 7BF3E0] { (220AA112D20) }
	typedef MonoDomain* (*MonoGetRootDomainFunc)();
	MonoGetRootDomainFunc getRootDomainFunc = nullptr;
	// Set the address of the function
	//The offset function mono_get_root_domain 
	//can found in from PE File Format 
	uintptr_t funcAddress = monoBaseAddress + 0xa4c30;  // Replace this with the correct address
	getRootDomainFunc = reinterpret_cast<MonoGetRootDomainFunc>(funcAddress);
	std::wcout << "mono domain from getRootDomainFunc: " << std::hex << getRootDomainFunc() << '\n';

	//Mem::setTargetProcess(gameProcHandle);
	//auto rootDomainAddr = Mem::readIntPtr(monoBaseAddress + 0x7BF3E0);
	//std::wcout << "mono root domain: " << std::hex << rootDomainAddr << '\n';
	while (true) {
		std::string input = "";
		std::cin >> input;
		if (input == "1") {
			break;
		}
	}
	FreeLibraryAndExitThread(m_hModule, 0);
}
BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	m_hModule = hModule;
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		//MessageBox(NULL, L"On Process Attach!", L"Hack Dll", MB_OK);
		myThreadHandle = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)launchHackThread, 0, 0, 0);
		break;
	case DLL_THREAD_ATTACH:
		//MessageBox(NULL, L"On Thread Attach!", L"Hack Dll", MB_OK);
		break;
	case DLL_THREAD_DETACH:
		//MessageBox(NULL, L"On Thread Detach", L"Hack Dll", MB_OK);
		break;
	case DLL_PROCESS_DETACH:
		//MessageBox(NULL, L"Process Detach l!", L"Hack Dll", MB_OK);
		break;
	}
	return TRUE;
}


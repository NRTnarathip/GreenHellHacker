#include "Headers.h"
#include "DLLInjector.h"
#include <conio.h>
#include <unordered_map>

void printAllFunctions() {
	cout << R"""(
1 : Injection Dll Green Hell
2 : test
)""";
}
int main(int argc, char* argv[]) {
	cout << EchoTextLaunch endline;
	cout << "\n";
	printAllFunctions();
	while (true) {
		string input = "";
		cout << "[CMD]: ";
		cin >> input;
		if (input == "1") {
			launchHackGame();
		}
	}
	printf("Exit App.");
}

int launchHackGame()
{
	auto gameName = (LPCWSTR)L"GH.exe";
	const wchar_t* monoDllName = L"mono-2.0-bdwgc.dll";
	intptr gameProcID = ProcessHack::getProcessIdByName(gameName);
	if (gameProcID == 0) {
		cout << "Error!! not found game\n";
		return 0;
	}
	HANDLE gameProcHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, gameProcID);
	Mem::setTargetProcess(gameProcHandle);

	HMODULE handleModules[1024];
	DWORD moduleReaded;
	intptr gameProcBaseAddress = 0;
	EnumProcessModules(gameProcHandle, handleModules, sizeof(handleModules), &moduleReaded);
	if (moduleReaded / sizeof(HMODULE) > 0) {
		gameProcBaseAddress = (intptr)handleModules[0];
	}
	intptr monoBaseAddress = 0;
	MODULEINFO* monoModuleInfo;
	std::unordered_map<std::wstring, HMODULE> mapModule;

	for (int i = 0; i < (moduleReaded / sizeof(HMODULE)); i++)
	{
		MODULEINFO moduleInfoBuffer;
		wchar_t szModName[MAX_PATH];
		auto moduleHandle = handleModules[i];
		GetModuleInformation(gameProcHandle, moduleHandle,
			&moduleInfoBuffer,
			sizeof(MODULEINFO));

		auto moduleName = GetModuleBaseNameW(gameProcHandle, moduleHandle,
			szModName, sizeof(szModName) / sizeof(wchar_t));

		//maping modules
		mapModule[szModName] = moduleHandle;

		//std::wcout << szModName << " < Module name: " << " handle: " << moduleHandle << "\n";
		if (std::wstring(szModName).compare(monoDllName) == 0) {
			monoModuleInfo = &moduleInfoBuffer;
			monoBaseAddress = reinterpret_cast<uintptr_t>(moduleInfoBuffer.lpBaseOfDll);
		}
	}

	cout << "Console App Base Address: " << std::hex << ProcessHack::getProcessBaseAddress(GetCurrentProcessId()) << "\n";
	cout << "Game Process base address: " << std::hex << gameProcBaseAddress << std::endl;
	cout << "mono-2.0-bwgc.dll base address " << std::hex << monoBaseAddress << '\n';

	// decode date time  23:13 7/8/2556
	// in Cheat Engine
	//mono-2.0-bdwgc.dll + A4C30 || 48 8B 05 A9A77100 || mov rax, [mono - 2.0 - bdwgc.dll + 7BF3E0] { (220AA112D20) }
	Mem::setTargetProcess(gameProcHandle);
	auto rootDomainAddr = Mem::readIntPtr(monoBaseAddress + 0x7BF3E0);

	//Injector
	auto doesInjected = DLLInjector::inject(gameProcHandle, mapModule[L"KERNEL32.DLL"],
		//"C:/Users/narat/Desktop/Hack/Green Hell Hack Tools/GreenHellHacker/x64/Release/HackDLL.dll");
		"HackDLL.dll");

	CloseHandle(gameProcHandle);
	return 0;
}

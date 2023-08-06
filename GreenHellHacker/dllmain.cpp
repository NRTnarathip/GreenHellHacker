// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <Windows.h>
#include <iostream>
#include <string>
#include <sstream>

typedef void(__cdecl* FuncA)();


DWORD WINAPI MainThread(LPVOID param) {
    printf("start main thread\n");

    int msgboxID = MessageBox(
        NULL,
        (LPCWSTR)L"GreenHell learning hack game", (LPCWSTR)L"Inject Succes",
        MB_OK | MB_ICONINFORMATION
    );
    //main game logic
    while (true) {
         
    }
    FreeLibraryAndExitThread((HMODULE)param, 0);
    printf("end main thread\n");
    return 0;
}
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        CreateThread(0, 0, MainThread, hModule, 0, 0);
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}


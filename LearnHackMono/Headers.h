#pragma once
#include <windows.h>
#include <iostream>
#include <psapi.h>
#include <string>
#include <fstream>
#include <format>
#include <TlHelp32.h>
#include <stdio.h>
#include <tchar.h>
#include <tlhelp32.h>
#include <codecvt>
#include "ProcessHack.h"
#include <netsh.h>

//my lib
#include "MemRead.hpp"
#include "Mono.h"

using namespace std;
#define endline << '\n'
#define intptr uintptr_t

auto EchoTextLaunch = R"""(
==============================================
           Reverse Engineer Tool
==============================================
Software Made by NRTnarathip
Youtube: https://www.youtube.com/@NRTnarathip
==============================================

)""";



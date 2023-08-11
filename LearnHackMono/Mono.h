#pragma once
#include <iostream>

struct MonoDomain
{
	char empty_1[0x30 - 0x1];
	uintptr_t* setup;
	uintptr_t* domain;
};

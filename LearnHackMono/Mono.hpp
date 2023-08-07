struct mono_root_domain_t {
	OFFSET(domain_assemblies(), glist_t*, 0xC8)
		OFFSET(domain_id(), int, 0xBC)
		OFFSET(jitted_function_table(), uintptr_t, 0x148)
};

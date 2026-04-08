/*
 * spitbol_noconv.c — fixture C library for net-ext-noconv tests.
 *
 * These functions receive NOCONV (unconverted) SNOBOL4 objects from the
 * DOTNET runtime as raw GCHandle-pinned pointers.  On the DOTNET side the
 * pinned address points to the managed Var subclass object on the heap.
 *
 * For test purposes we use a simpler convention: the SNOBOL4 program passes
 * an INTEGER or STRING value alongside the opaque block so we can exercise
 * the noconv marshaling path without needing to decode the full managed
 * object layout in C.  The "block" pointer is passed but not dereferenced
 * in these fixtures; what matters is that it arrives non-null.
 *
 * Functions exported:
 *   long   snc_noconv_nonnull(void *blk)          -- returns 1 if ptr != NULL
 *   long   snc_noconv_with_int(void *blk, long n) -- returns n+1 if ptr != NULL
 *   long   snc_array_passed(void *blk)            -- alias for nonnull, named
 *                                                    for array-passing tests
 *   long   snc_table_passed(void *blk)            -- alias for nonnull, named
 *                                                    for table-passing tests
 *                                                    
 *  Build (Windows):
 *  cl.exe /nologo /Od /Zi /MDd /D_DEBUG /Fe:libspitbol_noconv.dll spitbol_noconv.c /link /DLL /MACHINE:X64 /DEBUG /PDB:libspitbol_noconv.pdb
 *
 */

#include <stddef.h>   /* NULL */

#ifdef _WIN32
    #define EXPORT __declspec(dllexport)
#else
    #define EXPORT __attribute__((visibility("default")))
#endif

EXPORT long snc_noconv_nonnull(void *blk) {
    return blk != NULL ? 1 : 0;
}

EXPORT long snc_noconv_with_int(void *blk, long n) {
    return blk != NULL ? n + 1 : -1;
}
	
EXPORT long snc_array_passed(void *blk) {
    return blk != NULL ? 1 : 0;
}

EXPORT long snc_table_passed(void *blk) {
    return blk != NULL ? 1 : 0;
}

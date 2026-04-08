/*
 * spitbol_xn.c — fixture C library for net-ext-xnblk + net-load-xn tests.
 *
 * Windows (from x64-initialised prompt — see build.bat):
 *    cmd /c "`"$vcvars`" && cd /d `"$dir`" && cl.exe /nologo /Od /Zi /MDd /D_DEBUG /Fe:libspitbol_xn.dll spitbol_xn.c /link /DLL /MACHINE:X64 /DEBUG /PDB:libspitbol_xn.pdb"
 *
 * Linux:
 *   gcc -shared -fPIC -O0 -g -o libspitbol_xn.so spitbol_xn.c
 */

// MSVC doesn't implicitly define it, so the compiler treats every Null(string) as int 0, 
// causing both the "undeclared identifier" errors and all the indirection-mismatch warnings that follow.
#include <stddef.h>   /* NULL */

/* Cross-platform export macro */
#ifdef _WIN32
    #define EXPORT __declspec(dllexport)
#else
    #define EXPORT __attribute__((visibility("default")))
#endif


/* ── Runtime bridge ──────────────────────────────────────────────────────
 *
 * On Linux/macOS the snobol4_* symbols are resolved from the host process
 * symbol table at dlopen time (RTLD_GLOBAL).  On Windows that mechanism
 * does not exist: the .NET host calls snobol4_rt_register() immediately
 * after loading the DLL and passes the two shim function-pointers.
 *
 * Signature must match Load.cs:
 *   RtGetContextDelegate  → void (long **xndta_out, int *first_call_out)
 *   RtSetCallbackDelegate → void (void (*callback)(void))
 * ──────────────────────────────────────────────────────────────────────── */

typedef void (*get_context_fn)(long **xndta_out, int *first_call_out);
typedef void (*set_callback_fn)(void (*callback)(void));

static get_context_fn  _rt_get_context  = NULL;
static set_callback_fn _rt_set_callback = NULL;

EXPORT void snobol4_rt_register(get_context_fn get_ctx, set_callback_fn set_cb)
{
    _rt_get_context  = get_ctx;
    _rt_set_callback = set_cb;
}

/* ── Runtime shims (cross-platform — call through registered pointers) ── */

static long *snobol4_xndta(void)
{
    long *p = NULL;
    if (_rt_get_context) _rt_get_context(&p, NULL);
    return p;
}

static int snobol4_first_call(void)
{
    int fc = 0;
    if (_rt_get_context) _rt_get_context(NULL, &fc);
    return fc;
}

static void snobol4_register_callback(void (*fn)(void))
{
    if (_rt_set_callback) _rt_set_callback(fn);
}


/* ── xn_counter ─────────────────────────────────────────────────────── */

EXPORT long xn_counter(void)
{
    long *xndta = snobol4_xndta();
    if (snobol4_first_call())
        xndta[0] = 0;
    xndta[0]++;
    return xndta[0];
}

/* ── xn_first_call_flag ─────────────────────────────────────────────── */

EXPORT long xn_first_call_flag(void)
{
    return snobol4_first_call() ? 1 : 0;
}

/* ── xncbp callback machinery ───────────────────────────────────────── */

static long xn_callback_count_val = 0;

static void xn_cleanup(void)
{
    xn_callback_count_val++;
}

EXPORT long xn_reset_callback_count(void)
{
    xn_callback_count_val = 0;
    return 0;
}

EXPORT long xn_register_callback(void)
{
    snobol4_register_callback(xn_cleanup);
    return 1;
}

EXPORT long xn_callback_count(void)
{
    return xn_callback_count_val;
}
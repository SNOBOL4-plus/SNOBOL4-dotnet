/*
 * snobol4_rt.c — libsnobol4_rt: SPITBOL external-function runtime helpers.
 *
 * Provides the C-side API that external C libraries use to access SNOBOL4
 * runtime state.  The .NET runtime registers a "context callback" once per
 * thread before dispatching a C external function.  The C library calls
 * snobol4_xndta() / snobol4_first_call() and this shim routes the request
 * through the callback to the live NativeEntry state.
 *
 * Registration protocol
 * ---------------------
 * The .NET runtime calls snobol4_rt_register() once — immediately after
 * loading libsnobol4_rt.so — to supply a function pointer of type:
 *
 *   void snobol4_rt_get_context(long **xndta_out, int *first_call_out);
 *
 * This callback is called with the current thread's xndta pointer and
 * first_call flag.  It reads them from the [ThreadStatic] _currentNativeEntry
 * set around every CallNativeFunction dispatch in Load.cs.
 *
 * Exported API (used by fixture C libraries)
 * -------------------------------------------
 *   long* snobol4_xndta(void)       — pointer to xndta[0] for current call
 *   int   snobol4_first_call(void)  — 1 on first-ever call, 0 thereafter
 *   int   snobol4_reload_call(void) — always 0 on DOTNET (no save/reload)
 */

#include <stddef.h>

/* ── callback type ────────────────────────────────────────────────────── */

typedef void (*snobol4_rt_get_context_fn)(void *xndta_out, void *first_call_out);

static snobol4_rt_get_context_fn _get_context = NULL;

/* ── registration (called once by .NET runtime after dlopen) ─────────── */

void snobol4_rt_register(snobol4_rt_get_context_fn fn)
{
    _get_context = fn;
}

/* ── internal helper: fetch current context ─────────────────────────── */

static void _fetch(long **xndta_out, int *first_call_out)
{
    if (_get_context)
        _get_context(xndta_out, first_call_out);
    else
    {
        if (xndta_out)     *xndta_out     = NULL;
        if (first_call_out) *first_call_out = 0;
    }
}

/* ── public API ──────────────────────────────────────────────────────── */

long *snobol4_xndta(void)
{
    long *xndta = NULL;
    _fetch(&xndta, NULL);
    return xndta;
}

int snobol4_first_call(void)
{
    int fc = 0;
    _fetch(NULL, &fc);
    return fc;
}

/* Always 0 on DOTNET — no image save/reload mechanism exists yet. */
int snobol4_reload_call(void)
{
    return 0;
}

/* ── allocation helpers (net-ext-create) ────────────────────────────────── */
/*
 * These helpers let a C external function allocate a simple heap block and
 * return it as an EXTERNAL pointer to SNOBOL4.  The block is a plain malloc
 * region — the C function owns its layout.  SNOBOL4 stores the pointer in an
 * ExternalVar and passes it back as NOCONV on subsequent calls.
 *
 * snobol4_alloc(n)        — allocate n bytes, zero-filled; return pointer
 * snobol4_alloc_longs(n)  — allocate n longs (8*n bytes), zero-filled
 * snobol4_free(p)         — release a block allocated by snobol4_alloc*
 */

#include <stdlib.h>
#include <string.h>

void *snobol4_alloc(long n)
{
    if (n <= 0) return NULL;
    void *p = malloc((size_t)n);
    if (p) memset(p, 0, (size_t)n);
    return p;
}

long *snobol4_alloc_longs(long n)
{
    return (long *)snobol4_alloc(n * (long)sizeof(long));
}

void snobol4_free(void *p)
{
    free(p);
}

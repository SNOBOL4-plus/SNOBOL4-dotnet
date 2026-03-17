/*
 * spitbol_create.c — fixture C library for net-ext-create tests.
 *
 * Demonstrates M-NET-EXT-CREATE: a C external function allocates a block of
 * opaque state and returns it as an EXTERNAL pointer.  SNOBOL4 stores that
 * pointer in an ExternalVar and passes it back as a NOCONV argument on
 * subsequent calls.
 *
 * Exported functions
 * ------------------
 *   void* create_counter(void)
 *       Allocates a 1-long block initialised to 0.
 *       Returns the block pointer (EXTERNAL).
 *
 *   long  bump_counter(void *blk)
 *       Increments blk[0] and returns the new value (INTEGER).
 *       blk is the pointer originally returned by create_counter.
 *
 *   long  read_counter(void *blk)
 *       Returns blk[0] without incrementing (INTEGER).
 *
 *   void* create_pair(long a, long b)
 *       Allocates a 2-long block {a, b}.
 *       Returns the block pointer (EXTERNAL).
 *
 *   long  pair_sum(void *blk)
 *       Returns blk[0] + blk[1] (INTEGER).
 *
 * Build:
 *   gcc -shared -fPIC -O0 spitbol_create.c -o libspitbol_create.so
 *
 * These functions do NOT need libsnobol4_rt.so — they use plain malloc.
 */

#include <stdlib.h>
#include <string.h>

/* ── counter ──────────────────────────────────────────────────────────── */

void *create_counter(void)
{
    long *p = (long *)malloc(sizeof(long));
    if (p) *p = 0;
    return p;
}

long bump_counter(void *blk)
{
    long *p = (long *)blk;
    return ++(*p);
}

long read_counter(void *blk)
{
    return *((long *)blk);
}

/* ── pair ─────────────────────────────────────────────────────────────── */

void *create_pair(long a, long b)
{
    long *p = (long *)malloc(2 * sizeof(long));
    if (p) { p[0] = a; p[1] = b; }
    return p;
}

long pair_sum(void *blk)
{
    long *p = (long *)blk;
    return p[0] + p[1];
}

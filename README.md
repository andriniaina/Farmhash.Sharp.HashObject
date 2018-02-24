
hash an object multiple times superfast without doing reflection each time (experimental)

Farmhash.Sharp.HashObject
======

High performance generic memberwise GetHashCode() :
* internally uses Farmhash.Sharp for a superfast hashing function
* maintains a cache of expression tree => the Reflection is done only once per type => a magnitude times faster than using reflection

Supports :
* all primitive types
* Objects and nested objects (except circular references)
* enums

Performance on my machine (Athlon X4-860k)
======

| object count | time with expression tree cache | no cache                  |
|--------------|---------------------------------|---------------------------|
| 1            | **<1ms**                        | <1ms                      |
| 1000         | **1ms**                         | 601ms                     |
| 10000        | **14ms**                        | >6000ms (**425x slower**) |


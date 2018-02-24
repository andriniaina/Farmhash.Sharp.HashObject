

# Farmhash.Sharp.HashObject
High performance generic GetHashCode(), like Farmhash.Sharp but also works with objects! (experimental)

Internally uses a cache of expression tree => the Reflection is done only once per type => a magnitude times faster than using reflection

Performance on my machine (Athlon X4-860k)
======

| object count | time with expression tree cache | no cache                  |
|--------------|---------------------------------|---------------------------|
| 1            | **<1ms**                        | <1ms                      |
| 1000         | **1ms**                         | 601ms                     |
| 10000        | **14ms**                        | >6000ms (**425x slower**) |


> Copyright 2018 [andriniaina](https://github.com/andriniaina)
> 
> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
> 
> The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


hash an object multiple times superfast without doing reflection each time (experimental)

Farmhash.Sharp.HashObject
======

High performance generic memberwise GetHashCode() :
* internally uses Farmhash.Sharp for a superfast hashing function
* maintains a cache of expression tree => the Reflection is done only once per type => a magnitude times faster than using reflection

This library could have been written using functional programming an yielding debuggable dynamically built functions, but i am convinced that using Expression trees gives the greatest performance and avoids unnecessary type casts.

Supports :
* all primitive types
* Objects and nested objects (except circular references)
* IEnumerables
* enums

Performance on my machine (Athlon X4-860k)
======

| object count | time with expression tree cache | no cache                  |
|--------------|---------------------------------|---------------------------|
| 1            | **<1ms**                        | <1ms                      |
| 1000         | **1ms**                         | 601ms                     |
| 10000        | **14ms**                        | >6000ms (**425x slower**) |


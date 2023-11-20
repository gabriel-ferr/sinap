using JSON;
using Wavelets;
using Plots;

n = 2^11;
x0 = testfunction(n,"Doppler")
x = x0 + 0.05*randn(n)
y = denoise(x,TI=true)

t = range(0, 4; length=2048)

println(typeof(x))
plot(t, y)
#ifndef UTILITIES_CGINC
#define UTILITIES_CGINC

float white_noise(float3 vec, float3 seed = float3(32.1543, 19.63145, 43.1341))
{
    float randomVal = frac(sin(dot(sin(vec), seed) * 52761.92763));
    
    return randomVal;
}

float2 white_noise2d(float3 vec)
{
    float2 randomVal = float2(
        white_noise(vec, float3(15.9523, 72.77151, 23.4141)),
        white_noise(vec, float3(88.55312, 33.33217, 81.76512))
        );
    
    return randomVal;
}

float3 white_noise3d(float3 vec, float3 seed = float3(32.1543, 19.63145, 43.1341))
{
    float3 randomVal = float3(
        white_noise2d(vec),
        white_noise(vec, float3(131.1876, 11.1111, 53.51423))
    );
}

#endif

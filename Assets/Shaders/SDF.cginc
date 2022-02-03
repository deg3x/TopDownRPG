#ifndef UTILITIES_CGINC
#define UTILITIES_CGINC

float circle_sdf(float2 pos, float2 center, float radius)
{
    return length(pos - center) - radius;
}

float line_segment_sdf(float2 pos, float2 pointA, float2 pointB)
{
    float2 ap = pos - pointA;
    float2 ab = pointB - pointA;

    float lineInterp = clamp(dot(ap, ab)/dot(ab, ab), 0.0, 1.0);
    
    return length(ap - ab * lineInterp);
}

#endif

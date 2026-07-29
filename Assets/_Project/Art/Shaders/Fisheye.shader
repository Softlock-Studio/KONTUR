Shader "Hidden/KONTUR/Fisheye"
{
    Properties
    {
        _Strength ("Barrel Strength", Range(0, 2)) = 0.6
        _Strength2 ("Barrel Strength (outer)", Range(0, 2)) = 0.25
        _Zoom ("Zoom (hide black edges)", Range(1, 2)) = 1.15
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.05)) = 0.01
        _VignetteAmount ("Vignette Amount", Range(0, 1)) = 0.35
        _VignetteColor ("Vignette Color", Color) = (0, 0, 0, 1)

        _NoiseSpeed ("Distortion Noise Speed", Float) = 30.0
        _NoiseClampNum ("Distortion Clamp", Range(0, 1)) = 1.0
        _NoiseWaveNum ("Distortion Wave Count", Float) = 32.0
        _NoisePower ("Distortion Power", Range(0, 0.5)) = 0.05

        _StaticNoiseScale ("Static Noise Scale", Float) = 100.0
        _StaticNoiseBlend ("Static Noise Blend", Range(0, 1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "Fisheye"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Strength;
            float _Strength2;
            float _Zoom;
            float _ChromaticAberration;
            float _VignetteAmount;
            half4 _VignetteColor;
            float _Aspect; // set per-camera from FisheyeRendererFeature; _BlitTextureSize is not populated by AddBlitPass
            
            float _NoiseSpeed;
            float _NoiseClampNum;
            float _NoiseWaveNum;
            float _NoisePower;

            float _StaticNoiseScale;
            float _StaticNoiseBlend;

            // Ported from "GLSL Noise Algorithms" (patriciogonzalezvivo,
            // https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83).
            float Mod289(float x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float4 Mod289(float4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float4 Perm(float4 x) { return Mod289(((x * 34.0) + 1.0) * x); }

            float StaticNoise(float3 p, float scale)
            {
                p *= scale;
                float3 a = floor(p);
                float3 d = p - a;
                d = d * d * (3.0 - 2.0 * d);

                float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
                float4 k1 = Perm(b.xyxy);
                float4 k2 = Perm(k1.xyxy + b.zzww);

                float4 c = k2 + a.zzzz;
                float4 k3 = Perm(c);
                float4 k4 = Perm(c + 1.0);

                float4 o1 = frac(k3 * (1.0 / 41.0));
                float4 o2 = frac(k4 * (1.0 / 41.0));

                float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
                float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);

                return o4.y * d.y + o4.x * (1.0 - d.y);
            }

            // Maps a screen UV through GoPro-style radial (barrel) lens distortion.
            // Distortion is computed in aspect-corrected, zoomed space so the curve
            // reads as a true round lens rather than an oval stretched to the screen.
            float2 DistortUV(float2 uv, float aspect)
            {
                float2 centered = (uv - 0.5) * 2.0;   // -1..1
                centered.x *= aspect;

                float2 zoomed = centered / _Zoom;
                float r2 = dot(zoomed, zoomed);
                float2 distorted = zoomed * (1.0 + _Strength * r2 + _Strength2 * r2 * r2);

                distorted.x /= aspect;
                return distorted * 0.5 + 0.5;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float noise = frac(_Time * 1000 * _NoiseSpeed) - (1.0 - uv.y);
                uv.x += sin(radians(clamp(noise * 360.0 * _NoiseWaveNum, 0.0, 360.0 * _NoiseClampNum))) * _NoisePower;

                float2 uvR = DistortUV(uv + float2(_ChromaticAberration, 0), _Aspect);
                float2 uvG = DistortUV(uv, _Aspect);
                float2 uvB = DistortUV(uv - float2(_ChromaticAberration, 0), _Aspect);

                bool outOfBounds = any(uvG < 0.0) || any(uvG > 1.0);

                // Noise is sampled at the already-distorted UV (uvG) so it warps through the
                // same barrel-distortion field as the image, instead of sitting static on top.
                float n = StaticNoise(float3(uvG.x, uvG.y, _Time.y), _StaticNoiseScale);

                half r = lerp(SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uvR, 0).r, n, _StaticNoiseBlend);
                half g = lerp(SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uvG, 0).g, n, _StaticNoiseBlend);
                half b = lerp(SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uvB, 0).b, n, _StaticNoiseBlend);
                half4 col = half4(r, g, b, 1);

                if (outOfBounds)
                {
                    col = _VignetteColor;
                }

                float vignette = saturate(1.0 - dot(input.texcoord - 0.5, input.texcoord - 0.5) * 4.0 * _VignetteAmount);
                col.rgb = lerp(_VignetteColor.rgb, col.rgb, vignette);

                return col;
            }
            ENDHLSL
        }
    }
}

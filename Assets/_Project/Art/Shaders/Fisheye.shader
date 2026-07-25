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
                float2 uvR = DistortUV(input.texcoord + float2(_ChromaticAberration, 0), _Aspect);
                float2 uvG = DistortUV(input.texcoord, _Aspect);
                float2 uvB = DistortUV(input.texcoord - float2(_ChromaticAberration, 0), _Aspect);

                bool outOfBounds = any(uvG < 0.0) || any(uvG > 1.0);

                half r = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uvR, 0).r;
                half g = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uvG, 0).g;
                half b = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uvB, 0).b;
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

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace CameraSystem.Rendering
{
    // Full-screen GoPro-style barrel distortion plus static-noise blend, restricted to
    // cameras carrying a FisheyeLensMarker. Shares the URP Renderer asset with Main
    // Camera / Map Camera, which are left untouched.
    public class FisheyeRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private Material _material;
        [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        private FisheyePass _pass;

        public override void Create()
        {
            _pass = new FisheyePass
            {
                renderPassEvent = _renderPassEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_material == null)
            {
                return;
            }

            if (!renderingData.cameraData.camera.TryGetComponent(out FisheyeLensMarker marker))
            {
                return;
            }

            _pass.Setup(_material, marker.NoiseBlend);
            renderer.EnqueuePass(_pass);
        }

        private class FisheyePass : ScriptableRenderPass
        {
            private const string PassName = "Fisheye";
            private static readonly int AspectId = Shader.PropertyToID("_Aspect");
            private static readonly int StaticNoiseBlendId = Shader.PropertyToID("_StaticNoiseBlend");

            private Material _material;
            private float _staticNoiseBlend;
            private readonly MaterialPropertyBlock _propertyBlock = new();

            public void Setup(Material material, float staticNoiseBlend)
            {
                _material = material;
                _staticNoiseBlend = staticNoiseBlend;
                requiresIntermediateTexture = true;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();
                if (resourceData.isActiveTargetBackBuffer)
                {
                    return;
                }

                var cameraData = frameData.Get<UniversalCameraData>();

                TextureHandle source = resourceData.activeColorTexture;

                TextureDesc destinationDesc = renderGraph.GetTextureDesc(source);
                destinationDesc.name = $"CameraColor-{PassName}";
                destinationDesc.clearBuffer = false;

                TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

                // BlitMaterialParameters does not populate _BlitTextureSize/_BlitTexture_TexelSize,
                // so the shader can't derive aspect ratio from those. Feed it explicitly instead.
                _propertyBlock.SetFloat(AspectId, cameraData.camera.pixelWidth / (float)cameraData.camera.pixelHeight);
                _propertyBlock.SetFloat(StaticNoiseBlendId, _staticNoiseBlend);

                var blitParams = new RenderGraphUtils.BlitMaterialParameters(
                    source, destination, Vector2.one, Vector2.zero, _material, 0,
                    _propertyBlock, RenderGraphUtils.FullScreenGeometryType.ProceduralTriangle);
                renderGraph.AddBlitPass(blitParams, passName: PassName);

                resourceData.cameraColor = destination;
            }
        }
    }
}

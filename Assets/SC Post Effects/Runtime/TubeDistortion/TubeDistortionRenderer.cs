#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class TubeDistortionRenderer : ScriptableRendererFeature
    {
        class TubeDistortionRenderPass : PostEffectRenderer<TubeDistortion>
        {
            public TubeDistortionRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.TubeDistortion;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<TubeDistortion>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                Material.SetFloat("_Amount", volumeSettings.amount.value);

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, (int)volumeSettings.mode.value);
            }
        }

        TubeDistortionRenderPass m_ScriptablePass;
        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings(false);
        
        public override void Create()
        {
            m_ScriptablePass = new TubeDistortionRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
#endif
}
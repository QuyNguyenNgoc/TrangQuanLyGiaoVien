using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Hinnova.Authorization;

namespace Hinnova
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(HinnovaApplicationSharedModule),
        typeof(HinnovaCoreModule)
        )]
    public class HinnovaApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HinnovaApplicationModule).GetAssembly());
        }
    }
}
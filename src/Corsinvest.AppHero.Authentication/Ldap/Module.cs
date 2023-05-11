/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

//namespace Corsinvest.AppHero.Authentication.Ldap;

//public class Module : ModuleBase, IAutenticationConfig
//{
//    private IServiceCollection _services = default!;

//    public Module()
//    {
//        Authors = "Corsinvest Srl";
//        Company = "Corsinvest Srl"; 
//        Keywords = "Autentication,Ldap";
//        Category = IModularityService.AdministrationCategoryName;
//        Type = ModuleType.Authentication;

//        Link = new ModuleLink(this, Description)
//        {
//            Icon = UIIcon.Key.GetName(),
//            Enabled = false,
//        };
//    }

//    public override string Name => "AH.Ldap.Autentication";
//    public override string Description => "Ldap";
//    public AutenticationType AutenticationType => AutenticationType.Inline;
//    public override bool Configurated => _services.GetOptionsSnapshot<Options>().Value.Domains.Any(a => a.Enabled);

//    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
//    {
//        _services = services;
//        AddOptions<Options>(services, config);

//        services.AddScoped<IAutenticationLdap, AutenticationLdap>();
//    }
//}
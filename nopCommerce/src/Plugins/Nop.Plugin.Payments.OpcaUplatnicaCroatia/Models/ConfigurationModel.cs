using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.OpcaUplatnicaCroatia.Models;

public record ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
{
    public ConfigurationModel()
    {
        Locales = new List<ConfigurationLocalizedModel>();
    }

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.DescriptionText")]
    public string? DescriptionText { get; set; }
    public bool DescriptionText_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.AdditionalFee")]
    public decimal AdditionalFee { get; set; }
    public bool AdditionalFee_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.AdditionalFeePercentage")]
    public bool AdditionalFeePercentage { get; set; }
    public bool AdditionalFeePercentage_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.ShippableProductRequired")]
    public bool ShippableProductRequired { get; set; }
    public bool ShippableProductRequired_OverrideForStore { get; set; }
    
    
    [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.PrimateljPrvaLinija")]
    public string PrimateljPrvaLinija { get; set; } = "PRIMATELJ";
    public bool PrimateljPrvaLinija_OverrideForStore { get; set; }
    /// <summary>
    /// Max length 25
    /// </summary>
    /// 
    [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.PrimateljDrugaLinija")]
    public string PrimateljDrugaLinija { get; set; } = "PRIMATELJ ADRESA";
    public bool PrimateljDrugaLinija_OverrideForStore { get; set; } 
    /// <summary>
    /// Max length 27
    /// </summary>
    [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.PrimateljTrecaLinija")]
    public string PrimateljTrecaLinija { get; set; } = "PRIMATELJ MJESTO";
    public bool PrimateljTrecaLinija_OverrideForStore { get; set; }

    /// <summary>
    /// Max length 21
    /// </summary>
    [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.PrimateljIBAN")]
    public string PrimateljIBAN { get; set; } = "HR0000000000000000000";
    public bool PrimateljIBAN_OverrideForStore { get; set; }
    public IList<ConfigurationLocalizedModel> Locales { get; set; }

    #region Nested class

    public class ConfigurationLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.Payment.OpcaUplatnicaCroatia.DescriptionText")]
        public string? DescriptionText { get; set; }
    }

    #endregion

}
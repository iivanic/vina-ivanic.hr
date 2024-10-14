using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.OpcaUplatnicaCroatia;
using Nop.Plugin.Payments.OpcaUplatnicaCroatia.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.OpcaUplatnicaCroatia.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class PaymentOpcaUplatnicaCroatiaController : BasePaymentController
{
    #region Fields

    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public PaymentOpcaUplatnicaCroatiaController(ILanguageService languageService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _languageService = languageService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
            return AccessDeniedView();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var opcaUplatnicaCroatiaPaymentSettings = await _settingService.LoadSettingAsync<OpcaUplatnicaCroatiaPaymentSettings>(storeScope);

        var model = new ConfigurationModel
        {
            DescriptionText = opcaUplatnicaCroatiaPaymentSettings.DescriptionText
        };

        //locales
        await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
        {
            locale.DescriptionText = await _localizationService
                .GetLocalizedSettingAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
        });
        model.AdditionalFee = opcaUplatnicaCroatiaPaymentSettings.AdditionalFee;
        model.AdditionalFeePercentage = opcaUplatnicaCroatiaPaymentSettings.AdditionalFeePercentage;
        model.ShippableProductRequired = opcaUplatnicaCroatiaPaymentSettings.ShippableProductRequired;
        model.PrimateljPrvaLinija = opcaUplatnicaCroatiaPaymentSettings.PrimateljPrvaLinija;
        model.PrimateljDrugaLinija = opcaUplatnicaCroatiaPaymentSettings.PrimateljDrugaLinija;
        model.PrimateljTrecaLinija = opcaUplatnicaCroatiaPaymentSettings.PrimateljTrecaLinija;
        model.PrimateljIBAN = opcaUplatnicaCroatiaPaymentSettings.PrimateljIBAN;

        model.ActiveStoreScopeConfiguration = storeScope;
        if (storeScope > 0)
        {
            model.DescriptionText_OverrideForStore = await _settingService.SettingExistsAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.DescriptionText, storeScope);
            model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.AdditionalFee, storeScope);
            model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            model.ShippableProductRequired_OverrideForStore = await _settingService.SettingExistsAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.ShippableProductRequired, storeScope);
            model.PrimateljPrvaLinija_OverrideForStore = await _settingService.SettingExistsAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.PrimateljPrvaLinija, storeScope);
            model.PrimateljDrugaLinija_OverrideForStore = await _settingService.SettingExistsAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.PrimateljDrugaLinija, storeScope);
            model.PrimateljTrecaLinija_OverrideForStore = await _settingService.SettingExistsAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.PrimateljTrecaLinija, storeScope);
            model.PrimateljIBAN_OverrideForStore = await _settingService.SettingExistsAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.PrimateljIBAN, storeScope);
        }

        return View("~/Plugins/Payments.OpcaUplatnicaCroatia/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var opcaUplatnicaCroatiaPaymentSettings = await _settingService.LoadSettingAsync<OpcaUplatnicaCroatiaPaymentSettings>(storeScope);

        //save settings
        opcaUplatnicaCroatiaPaymentSettings.DescriptionText = model.DescriptionText;
        opcaUplatnicaCroatiaPaymentSettings.AdditionalFee = model.AdditionalFee;
        opcaUplatnicaCroatiaPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
        opcaUplatnicaCroatiaPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;
        opcaUplatnicaCroatiaPaymentSettings.PrimateljPrvaLinija = model.PrimateljPrvaLinija;
        opcaUplatnicaCroatiaPaymentSettings.PrimateljDrugaLinija = model.PrimateljDrugaLinija;
        opcaUplatnicaCroatiaPaymentSettings.PrimateljTrecaLinija = model.PrimateljTrecaLinija;
        opcaUplatnicaCroatiaPaymentSettings.PrimateljIBAN = model.PrimateljIBAN;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        await _settingService.SaveSettingOverridablePerStoreAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.PrimateljPrvaLinija, model.PrimateljPrvaLinija_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.PrimateljDrugaLinija, model.PrimateljDrugaLinija_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.PrimateljTrecaLinija, model.PrimateljTrecaLinija_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(opcaUplatnicaCroatiaPaymentSettings, x => x.PrimateljIBAN, model.PrimateljIBAN_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        //localization. no multi-store support for localization yet.
        foreach (var localized in model.Locales)
        {
            await _localizationService.SaveLocalizedSettingAsync(opcaUplatnicaCroatiaPaymentSettings,
                x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion
}
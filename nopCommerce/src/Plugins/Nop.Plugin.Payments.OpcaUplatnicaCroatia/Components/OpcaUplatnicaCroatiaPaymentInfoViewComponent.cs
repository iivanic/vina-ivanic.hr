using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.OpcaUplatnicaCroatia;
using Nop.Plugin.Payments.OpcaUplatnicaCroatia.Models;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.OpcaUplatnicaCroatia.Components;

public class OpcaUplatnicaCroatiaViewComponent : NopViewComponent
{
    protected readonly OpcaUplatnicaCroatiaPaymentSettings _opcaUplatnicaPaymentSettings;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly INotificationService _notificationService;
    protected readonly IPaymentService _paymentService;
    protected readonly OrderSettings _orderSettings;
    protected readonly IOrderService _orderService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IProductService _productService;
    protected readonly IShoppingCartService _shoppingCartService;

    public OpcaUplatnicaCroatiaViewComponent(OpcaUplatnicaCroatiaPaymentSettings opcaUplatnicaPaymentSettings,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        IWorkContext workContext,
        INotificationService notificationService,
        IPaymentService paymentService,
        OrderSettings orderSettings,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IProductService productService,
        IShoppingCartService shoppingCartService
        )
    {
        _opcaUplatnicaPaymentSettings = opcaUplatnicaPaymentSettings;
        _localizationService = localizationService;
        _storeContext = storeContext;
        _workContext = workContext;
        _notificationService = notificationService;
        _paymentService = paymentService;
        _orderSettings = orderSettings;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _productService = productService;
        _shoppingCartService = shoppingCartService;

    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {

        var store = await _storeContext.GetCurrentStoreAsync();

        var model = new PaymentInfoModel
        {
            DescriptionText = await _localizationService.GetLocalizedSettingAsync(_opcaUplatnicaPaymentSettings,
                x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, store.Id)
        };
        model.PrimateljIBAN = _opcaUplatnicaPaymentSettings.PrimateljIBAN;
        model.PrimateljPrvaLinija = _opcaUplatnicaPaymentSettings.PrimateljPrvaLinija;
        model.PrimateljDrugaLinija = _opcaUplatnicaPaymentSettings.PrimateljDrugaLinija;
        model.PrimateljTrecaLinija = _opcaUplatnicaPaymentSettings.PrimateljTrecaLinija;
        model.PlatiteljPrvaLinija = "A";
        model.PlatiteljDrugaLinija = "A";
        model.PlatiteljTrecaLinija = "A";
        model.Opis = "Opis";
    
        model.ValutaPlacanja = "EUR";
        model.Model = "HR99";
        model.SifraNamjene = "COST";
        model.PozivNaBroj = "123";

        var total = await _orderTotalCalculationService.GetShoppingCartTotalAsync(
           await _shoppingCartService.GetShoppingCartAsync( await _workContext.GetCurrentCustomerAsync())
            );
        model.Iznos = ((int)(total.shoppingCartTotal.Value* 100)).ToString();
        

        if (!System.IO.Directory.Exists("wwwroot/uplatnice"))
            System.IO.Directory.CreateDirectory("wwwroot/uplatnice");
        CleanOldUplatnice();

        Hub3a.Hub3a hub = new Hub3a.Hub3a();
        string fileName = $"wwwroot/uplatnice/uplatnica_{Guid.NewGuid().ToString()}.pdf";

        hub.Iznos = model.Iznos;
        hub.Opis = model.Opis;
        hub.Model = model.Model;
        hub.PrimateljPrvaLinija = model.PrimateljPrvaLinija;
        hub.PrimateljDrugaLinija = model.PrimateljDrugaLinija;
        hub.PrimateljTrecaLinija = model.PrimateljTrecaLinija;
        hub.PlatiteljPrvaLinija = model.PlatiteljPrvaLinija;
        hub.PlatiteljDrugaLinija = model.PlatiteljDrugaLinija;
        hub.PlatiteljTrecaLinija = model.PlatiteljTrecaLinija;
        hub.PozivNaBroj = model.PozivNaBroj;
        hub.PrimateljIBAN = model.PrimateljIBAN;
        hub.SifraNamjene = model.SifraNamjene;
        hub.ValutaPlacanja = model.ValutaPlacanja;

        hub.DajPDFUplatnicu(fileName);
        hub.Dispose();
        fileName = fileName.Replace("wwwroot/", "");
        model.fileName = fileName;
        return View("~/Plugins/Payments.OpcaUplatnicaCroatia/Views/PaymentInfo.cshtml", model);
    }
    private void CleanOldUplatnice()
    {
        foreach (var s in Directory.GetFiles("wwwroot/uplatnice"))
        {
            DateTime ft = File.GetCreationTime(s);
            if (DateTime.Now - ft > TimeSpan.FromDays(15))
            {
                File.Delete(s);
            }
        }
    }
}
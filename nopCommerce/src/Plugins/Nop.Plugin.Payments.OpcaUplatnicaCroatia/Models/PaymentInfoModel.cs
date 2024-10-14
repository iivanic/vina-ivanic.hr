using Microsoft.AspNetCore.SignalR;
using Nop.Web.Framework.Models;
using Nop.Plugin.Payments.OpcaUplatnicaCroatia.Hub3a;
namespace Nop.Plugin.Payments.OpcaUplatnicaCroatia.Models;

public record PaymentInfoModel : BaseNopModel
{
    public string fileName { get; set; }
    public string DescriptionText { get; set; }

    public string ValutaPlacanja { get; set; } 
    /// <summary>
    /// Max length 15
    /// </summary>
    public string Iznos { get; set; }

    /// <summary>
    /// Max length 30
    /// </summary>
    public string PlatiteljPrvaLinija { get; set; } 
    /// <summary>
    /// Max length 27
    /// </summary>
    public string PlatiteljDrugaLinija { get; set; }
    /// <summary>
    /// Max length 27
    /// </summary>
    public string PlatiteljTrecaLinija { get; set; } 
    /// <summary>
    /// Max length 25
    /// </summary>
    public string PrimateljPrvaLinija { get; set; }
    /// <summary>
    /// Max length 25
    /// </summary>
    public string PrimateljDrugaLinija { get; set; } 
    /// <summary>
    /// Max length 27
    /// </summary>
    public string PrimateljTrecaLinija { get; set; } 
    /// <summary>
    /// Max length 21
    /// </summary>
    public string PrimateljIBAN { get; set; }
    /// <summary>
    /// Max length 4
    /// </summary>
    public string Model { get; set; } 
    /// <summary>
    /// Max length 22
    /// </summary>
    public string PozivNaBroj { get; set; }
    /// <summary>
    /// Max length 4
    /// </summary>
    public string SifraNamjene { get; set; }
    /// <summary>
    /// Max length 35
    /// </summary>
    public string Opis { get; set; } 

}

 


﻿using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.OpcaUplatnicaCroatia;

/// <summary>
/// Represents settings of "Check money order" payment plugin
/// </summary>
public class OpcaUplatnicaCroatiaPaymentSettings : ISettings
{
    /// <summary>
    /// Gets or sets a description text
    /// </summary>
    public string DescriptionText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
    /// </summary>
    public bool AdditionalFeePercentage { get; set; }

    /// <summary>
    /// Gets or sets an additional fee
    /// </summary>
    public decimal AdditionalFee { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether shippable products are required in order to display this payment method during checkout
    /// </summary>
    public bool ShippableProductRequired { get; set; }
    public string PrimateljPrvaLinija { get; set; } = "PRIMATELJ";
    /// <summary>
    /// Max length 25
    /// </summary>
    public string PrimateljDrugaLinija { get; set; } = "PRIMATELJ ADRESA";
    /// <summary>
    /// Max length 27
    /// </summary>
    public string PrimateljTrecaLinija { get; set; } = "PRIMATELJ MJESTO";
    /// <summary>
    /// Max length 21
    /// </summary>
    public string PrimateljIBAN { get; set; } = "HR0000000000000000000";
}
/*
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */

using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace His_Pos.RDLC
{
    [XmlRoot(ElementName = "TextRun", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TextRun
    {
        [XmlElement(ElementName = "Value", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Value { get; set; }

        [XmlElement(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Style { get; set; }
    }

    [XmlRoot(ElementName = "TextRuns", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TextRuns
    {
        [XmlElement(ElementName = "TextRun", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TextRun TextRun { get; set; }
    }

    [XmlRoot(ElementName = "Paragraph", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Paragraph
    {
        [XmlElement(ElementName = "TextRuns", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TextRuns TextRuns { get; set; }

        [XmlElement(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Style Style { get; set; }
    }

    [XmlRoot(ElementName = "Paragraphs", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Paragraphs
    {
        [XmlElement(ElementName = "Paragraph", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Paragraph Paragraph { get; set; }
    }

    [XmlRoot(ElementName = "Visibility", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Visibility
    {
        [XmlElement(ElementName = "Hidden", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public bool Hidden { get; set; }
    }

    [XmlRoot(ElementName = "Border", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Border
    {
        [XmlElement(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Style { get; set; }
    }

    [XmlRoot(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Style
    {
        [XmlElement(ElementName = "Border", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Border Border { get; set; }

        [XmlElement(ElementName = "PaddingLeft", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string PaddingLeft { get; set; }

        [XmlElement(ElementName = "PaddingRight", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string PaddingRight { get; set; }

        [XmlElement(ElementName = "PaddingTop", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string PaddingTop { get; set; }

        [XmlElement(ElementName = "PaddingBottom", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string PaddingBottom { get; set; }

        [XmlElement(ElementName = "BackgroundImage", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public BackgroundImage BackgroundImage { get; set; }

        [XmlElement(ElementName = "TextAlign", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string TextAlign { get; set; }
    }

    [XmlRoot(ElementName = "Textbox", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Textbox
    {
        [XmlElement(ElementName = "CanGrow", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string CanGrow { get; set; }

        [XmlElement(ElementName = "KeepTogether", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string KeepTogether { get; set; }

        [XmlElement(ElementName = "Paragraphs", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Paragraphs Paragraphs { get; set; }

        [XmlElement(ElementName = "DefaultName", Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
        public string DefaultName { get; set; }

        [XmlElement(ElementName = "Top", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Top { get; set; }

        [XmlElement(ElementName = "Left", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Left { get; set; }

        [XmlElement(ElementName = "Height", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Height { get; set; }

        [XmlElement(ElementName = "Width", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Width { get; set; }

        [XmlElement(ElementName = "Visibility", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Visibility Visibility { get; set; }

        [XmlElement(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Style Style { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "TablixColumn", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixColumn
    {
        [XmlElement(ElementName = "Width", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Width { get; set; }
    }

    [XmlRoot(ElementName = "TablixColumns", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixColumns
    {
        [XmlElement(ElementName = "TablixColumn", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixColumn TablixColumn { get; set; }
    }

    [XmlRoot(ElementName = "Rectangle", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Rectangle
    {
        [XmlElement(ElementName = "KeepTogether", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string KeepTogether { get; set; }

        [XmlElement(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Style Style { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "CellContents", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class CellContents
    {
        [XmlElement(ElementName = "Rectangle", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Rectangle Rectangle { get; set; }
    }

    [XmlRoot(ElementName = "TablixCell", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixCell
    {
        [XmlElement(ElementName = "CellContents", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public CellContents CellContents { get; set; }
    }

    [XmlRoot(ElementName = "TablixCells", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixCells
    {
        [XmlElement(ElementName = "TablixCell", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixCell TablixCell { get; set; }
    }

    [XmlRoot(ElementName = "TablixRow", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixRow
    {
        [XmlElement(ElementName = "Height", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Height { get; set; }

        [XmlElement(ElementName = "TablixCells", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixCells TablixCells { get; set; }
    }

    [XmlRoot(ElementName = "TablixRows", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixRows
    {
        [XmlElement(ElementName = "TablixRow", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixRow TablixRow { get; set; }
    }

    [XmlRoot(ElementName = "TablixBody", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixBody
    {
        [XmlElement(ElementName = "TablixColumns", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixColumns TablixColumns { get; set; }

        [XmlElement(ElementName = "TablixRows", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixRows TablixRows { get; set; }
    }

    [XmlRoot(ElementName = "TablixMembers", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixMembers
    {
        [XmlElement(ElementName = "TablixMember", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixMember TablixMember { get; set; }
    }

    [XmlRoot(ElementName = "TablixColumnHierarchy", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixColumnHierarchy
    {
        [XmlElement(ElementName = "TablixMembers", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixMembers TablixMembers { get; set; }
    }

    [XmlRoot(ElementName = "Group", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Group
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "TablixMember", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixMember
    {
        [XmlElement(ElementName = "Group", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Group Group { get; set; }
    }

    [XmlRoot(ElementName = "TablixRowHierarchy", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class TablixRowHierarchy
    {
        [XmlElement(ElementName = "TablixMembers", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixMembers TablixMembers { get; set; }
    }

    [XmlRoot(ElementName = "Tablix", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Tablix
    {
        [XmlElement(ElementName = "TablixBody", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixBody TablixBody { get; set; }

        [XmlElement(ElementName = "TablixColumnHierarchy", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixColumnHierarchy TablixColumnHierarchy { get; set; }

        [XmlElement(ElementName = "TablixRowHierarchy", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public TablixRowHierarchy TablixRowHierarchy { get; set; }

        [XmlElement(ElementName = "Top", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Top { get; set; }

        [XmlElement(ElementName = "Left", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Left { get; set; }

        [XmlElement(ElementName = "Height", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Height { get; set; }

        [XmlElement(ElementName = "Width", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Width { get; set; }

        [XmlElement(ElementName = "ZIndex", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string ZIndex { get; set; }

        [XmlElement(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Style Style { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "ReportItems", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class ReportItems
    {
        [XmlElement(ElementName = "Textbox", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public ObservableCollection<Textbox> Textbox { get; set; } = new ObservableCollection<Textbox>();

        [XmlElement(ElementName = "Tablix", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Tablix Tablix { get; set; }
    }

    [XmlRoot(ElementName = "BackgroundImage", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class BackgroundImage
    {
        [XmlElement(ElementName = "Source", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Source { get; set; }

        [XmlElement(ElementName = "Value", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Body
    {
        [XmlElement(ElementName = "ReportItems", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public ReportItems ReportItems { get; set; }

        [XmlElement(ElementName = "Height", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Height { get; set; }

        [XmlElement(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Style Style { get; set; }
    }

    [XmlRoot(ElementName = "Page", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Page
    {
        [XmlElement(ElementName = "PageHeight", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string PageHeight { get; set; }

        [XmlElement(ElementName = "PageWidth", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string PageWidth { get; set; }

        [XmlElement(ElementName = "LeftMargin", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string LeftMargin { get; set; }

        [XmlElement(ElementName = "RightMargin", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string RightMargin { get; set; }

        [XmlElement(ElementName = "TopMargin", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string TopMargin { get; set; }

        [XmlElement(ElementName = "BottomMargin", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string BottomMargin { get; set; }

        [XmlElement(ElementName = "ColumnSpacing", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string ColumnSpacing { get; set; }

        [XmlElement(ElementName = "Style", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Style { get; set; }
    }

    [XmlRoot(ElementName = "Report", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
    public class Report
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Body Body { get; set; }

        [XmlElement(ElementName = "Width", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string Width { get; set; }

        [XmlElement(ElementName = "Page", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public Page Page { get; set; }

        [XmlElement(ElementName = "AutoRefresh", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string AutoRefresh { get; set; }

        [XmlElement(ElementName = "ReportUnitType", Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
        public string ReportUnitType { get; set; }

        [XmlElement(ElementName = "ConsumeContainerWhitespace", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
        public string ConsumeContainerWhitespace { get; set; }

        [XmlElement(ElementName = "ReportID", Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
        public string ReportID { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlAttribute(AttributeName = "rd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Rd { get; set; }
    }
}
/*
 * Created by SharpDevelop.
 * User: jochen
 * Date: 03.01.2011
 * Time: 20:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
	/// <summary>
	/// Description of S5ParameterFormat.
	/// </summary>
	public enum S5ParameterFormat
	{
		//Bei E o. A
		BI, //Bit
		BY, //Byte
		W,  //Wort
		D,  //Doppelwort
		
		//Bei D
		KM, 
		KH, 
		KY, 
		KC, 
		KF, 
		KT, 
		KZ, 
		KG, 
	}
}

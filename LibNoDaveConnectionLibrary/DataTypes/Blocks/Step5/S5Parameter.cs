/*
 * Created by SharpDevelop.
 * User: jochen
 * Date: 03.01.2011
 * Time: 20:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
	/// <summary>
	/// Description of S5Parameter.
	/// </summary>
	public class S5Parameter
	{
		public string Name {get; set;}
        public string Comment { get; set; }
        public string Value { get; set; }
		public S5ParameterFormat S5ParameterFormat {get; set;}
		public S5ParameterType S5ParameterType {get; set;}
		
		public override string ToString()
		{
			string retval=Name + ", " + S5ParameterType.ToString();
			if (S5ParameterType== S5ParameterType.E || S5ParameterType== S5ParameterType.A || S5ParameterType== S5ParameterType.D)
				retval += ", " + S5ParameterFormat.ToString();
			return retval;
		}


	}
}

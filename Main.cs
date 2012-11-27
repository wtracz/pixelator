using System;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Linq;

namespace ImagePixelator
{
	class MainClass
	{

		private static String GenerateHTMLForImage(Image img, IStrategy strategy) {

			// Create table .
			Table tbl = new Table();

			// Convert image.
			strategy.generateTable(img, tbl);

			// Table to HTML.
			HTMLBuilder builder = new HTMLBuilder();
			tbl.accept(builder);

			return builder.HTML;
		}

		public static void Main (string[] args)
		{
			// Some pre-processing filters.
			IFilter[] filters = new IFilter[] {
				new BMPConvert(),
				// new DynamicRangeReduce(),
			};

			// Find all strategies and instantiate.
			Type[] strategy_types = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(
					type => typeof(IStrategy).IsAssignableFrom(type) && typeof(IStrategy) != type
				)
				.ToArray();

			IStrategy[] strategies = new IStrategy[strategy_types.Length];
			for(int t = 0; t < strategy_types.Length; ++t) {
				strategies[t] = (IStrategy) Activator.CreateInstance(strategy_types[t]);
			}

			StringBuilder html = new StringBuilder();
			html.Append ("<html><body>");
			foreach(string arg in args) {

				// Load image from arguments.
				Image bmp = Image.FromFile(arg);

				// Run through filters.
				foreach(IFilter filter in filters) {
					bmp = filter.filter (bmp);
				}

				// Run through each strategy.
				html.AppendFormat ("<div><h2>{0}</h2>", arg);
				foreach(IStrategy strategy in strategies) {
					String table = GenerateHTMLForImage(bmp, strategy);

					html.AppendFormat ("<h3>{0}</h3><p>{1} bytes</p><div>{2}</div>", strategy.Name, table.Length, table);

				}
				html.Append ("</div>");

			}
			html.Append ("</body>");

			// Dump to console.
			Console.Write (html.ToString ());
		}
	}
}

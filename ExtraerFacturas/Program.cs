// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

string rutaListaDocs = AppDomain.CurrentDomain.BaseDirectory.ToString() + $@"\Datos\ListadoDocumentos.txt";
//Console.WriteLine("Ruta: "+rutaListaDocs);

string path = @"Datos\newListado.txt";
CreateNewlist(path);

//copiarDocumentos("pdf", "archivo1.pdf");

stractor();

//CSVToDataTable("Datos\\l.csv");

string listado =string.Empty;
string lineaAux= string.Empty;
//foreach (string line in System.IO.File.ReadLines("Datos\\List.txt"))
//{
//    lineaAux = string.Empty;
//    int count = 0;
//    if (line.IndexOf("-") > 0)
//    {
//        for (int i = line.IndexOf("-"); i <= line.Length; i++)
//        {
//           //Console.WriteLine(line[i]);
//            if (line[i] == '0' || line[i] == '-')
//            {
//                continue;
//            }
//            else
//            {
//                count = i;
//                break;
//            }
//        }
//        lineaAux = line.Substring(0, line.IndexOf("-")) + line.Substring(count);
//        listado += lineaAux + "\n";
//    }
//    else
//    {
//        listado += line + "\n";
//    }

//}
    //WriteInList(path, listado);







static void CreateNewlist(string path)
{
    using (FileStream fs = File.Create(path))
    {
        byte[] info = new UTF8Encoding(true).GetBytes("");
        fs.Write(info, 0, info.Length);
        fs.Close();
    }
}
static void WriteInList(string path, string line)
{
    using (StreamWriter writetext = new StreamWriter(path))
    {
        writetext.WriteLine(line);
    }
}

static DataTable CSVtoDataTable(string filepath)
{
    int count = 1;
    char fieldSeparator = ';';
    DataTable csvData = new DataTable();

    using (TextFieldParser csvReader = new TextFieldParser(filepath))
    {
        csvReader.HasFieldsEnclosedInQuotes = true;
        while (!csvReader.EndOfData)
        {
            csvReader.SetDelimiters(new string[] { ";" });
            string[] fieldData = csvReader.ReadFields();
            if (count == 0)
            {
                foreach (string column in fieldData)
                {
                    DataColumn datecolumn = new DataColumn(column);
                    datecolumn.AllowDBNull = true;
                    csvData.Columns.Add(datecolumn);
                }
            }
            else
            {
                csvData.Rows.Add(fieldData);
            }

        }
    }

    for(int i = 0; i<csvData.Rows.Count; i++)
    {
        Console.WriteLine(csvData.Rows[i].ToString());
    }
    return csvData;
}


static DataTable CSVToDataTable(string path)
{
    bool isHeaders = true;
    DataTable dt = new DataTable();

    foreach (string line in File.ReadLines(path))
    {
        
        string[] rowData = line.Split(';');
        if (isHeaders)
        {
            foreach(string title in rowData)
            {
                dt.Columns.Add(title.Trim());
            }
            isHeaders = false;
        }
        else
        {
            DataRow dr = dt.NewRow();

            for(int i = 0; i < rowData.Count(); i++)
            {
                dr[i] = rowData[i].ToString();
            }
            dt.Rows.Add(dr);
        }
    }
    //foreach (DataRow dr in dt.Rows)
    //{
    //    Console.WriteLine(dr["Pdf"]);
    //}
    return dt;
}

static bool copiarDocumentos(string typeDocument, string fileName)
{
    string pathInput = AppDomain.CurrentDomain.BaseDirectory.ToString() + $@"\Datos\fileInput\";
    string pathOutput = AppDomain.CurrentDomain.BaseDirectory.ToString() + $@"\Datos\fileOutput\";

    try
    {
        if (System.IO.File.Exists($@"{pathInput}\{typeDocument}\{fileName}"))
        {
            File.Copy($@"{pathInput}\{typeDocument}\{fileName}", $@"{pathOutput}\{typeDocument}\{fileName}", true);
            return true;
        }
        else
        {
            Console.WriteLine("Source path does not exist!");
            return false;
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine($"error en copiarDocumentos: {ex}");
        return false;
    }

}
static string[] findFileName(DataTable dt, string legalNumer)
{
    try
    {
        DataRow[] drFined = dt.Select($"NumLegal='{legalNumer}'");
        if (drFined==null || drFined.Count()<=0)
        {
            return null;
        }
        else
        {
            string[] namesDocumentos =new string[2];
            namesDocumentos[0] = drFined[0]["Pdf"].ToString();
            namesDocumentos[1] = drFined[0]["Xml"].ToString();
            return namesDocumentos;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"error en copiarDocumentos{ex}");
        return null;
    }
}

static void stractor()
{

    try
    {
        string docNoEnlistados = string.Empty;
        string docNoExist = string.Empty;
        DataTable dt = CSVToDataTable("Datos\\l.csv");

        foreach (string line in File.ReadLines("Datos\\newListado2.txt"))
        {
            //Console.WriteLine(line);
            string[] resultFined = findFileName(dt, line);
            if ( resultFined == null)
            {
                docNoEnlistados += line+"\n";
            }
            else
            {
                bool existePdf = copiarDocumentos("pdf", resultFined[0]);
                bool existeXml = copiarDocumentos("xml", resultFined[1]);
                if (!existePdf || !existeXml) 
                    docNoExist+= line +"\n";


            }
        }

        Console.WriteLine("Documentos no enlistados:\n"+ docNoEnlistados);
        Console.WriteLine("Documentos no éxisten:\n"+ docNoExist);
    }catch (Exception ex)
    {
        Console.WriteLine("Error en stractor" + ex);
    }
}
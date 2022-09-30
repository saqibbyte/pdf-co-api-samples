'*******************************************************************************************'
'                                                                                           '
' Download Free Evaluation Version From:     https://bytescout.com/download/web-installer   '
'                                                                                           '
' Also available as Web API! Get free API Key https://app.pdf.co/signup                     '
'                                                                                           '
' Copyright © 2017-2020 ByteScout, Inc. All rights reserved.                                '
' https://www.bytescout.com                                                                 '
' https://www.pdf.co                                                                        '
'*******************************************************************************************'


Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Module Module1

	' The authentication key (API Key).
	' Get your own by registering at https://app.pdf.co
	Const API_KEY As String = "***********************************"

	' Direct URL of source file to search barcodes in.
    ' You can also upload your own file into PDF.co and use it as url. Check "Upload File" samples for code snippets: https://github.com/bytescout/pdf-co-api-samples/tree/master/File%20Upload/   
	Const SourceFileURL As String = "https://bytescout-com.s3-us-west-2.amazonaws.com/files/demo-files/cloud-api/encryption/barcode_encrypted_aes128.png"

	' Comma-separated list of barcode types to search. 
	' See valid barcode types in the documentation https://apidocs.pdf.co
	Const BarcodeTypes As String = "QRCode"

	' Refer to documentations for more info. https://apidocs.pdf.co/32-1-user-controlled-data-encryption-and-decryption.
	Const Profiles As String = "{ 'DataDecryptionAlgorithm': 'AES128', 'DataDecryptionKey': 'Qweasd1234567890', 'DataDecryptionIV': '0mDI&qLv*ivTCd$*' }"

	Sub Main()

		' Create standard .NET web client instance
		Dim webClient As WebClient = New WebClient()

		' Set API Key
		webClient.Headers.Add("x-api-key", API_KEY)

        ' Set JSON content type
        webClient.Headers.Add("Content-Type", "application/json")

		' Prepare URL for `Barcode Reader` API call
		Dim url As String = "https://api.pdf.co/v1/barcode/read/from/url"

        ' Prepare requests params as JSON
        ' See documentation: https : //apidocs.pdf.co
        Dim parameters As New Dictionary(Of String, Object)
		parameters.Add("types", BarcodeTypes)
		parameters.Add("profiles", Profiles)
		parameters.Add("url", SourceFileURL)

        ' Convert dictionary of params to JSON
        Dim jsonPayload As String = JsonConvert.SerializeObject(parameters)

		Try
			' Execute POST request with JSON payload
			Dim response As String = webClient.UploadString(url, jsonPayload)

			' Parse JSON response
			Dim json As JObject = JObject.Parse(response)

			If json("error").ToObject(Of Boolean) = False Then
				
				' Display found barcodes in console
				For Each token As JToken In json("barcodes")
					Console.WriteLine("Found barcode:")
					Console.WriteLine("  Type: " + token("TypeName").ToString())
					Console.WriteLine("  Value: " + token("Value").ToString())
					Console.WriteLine("  Document Page Index: " + token("Page").ToString())
					Console.WriteLine("  Rectangle: " + token("Rect").ToString())
					Console.WriteLine("  Confidence: " + token("Confidence").ToString())
					Console.WriteLine()
				Next

			Else 
				Console.WriteLine(json("message").ToString())
			End If
			
		Catch ex As WebException
			Console.WriteLine(ex.ToString())
		End Try

		webClient.Dispose()


		Console.WriteLine()
		Console.WriteLine("Press any key...")
		Console.ReadKey()

	End Sub

End Module

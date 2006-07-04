//
// IHtmlRender.cs: Interface that abstracts the html render widget
// Author: Mario Sopena
//
// This code is based on an IHtmlRenderer interface in the monodoc-browser project
using System;
using Gtk;

namespace Beline.Tools {
public interface IHtmlRender {
  /// Event fired when the submit button is clicked
  event SubmitHandler OnSubmit;
	/// Event fired when the use is over an Url
	event EventHandler OnUrl;
	/// Event fired when the user clicks on a Link
	event EventHandler UrlClicked;
  
	// Jump to an anchor of the form <a name="tttt">
	void JumpToAnchor (string anchor_name);

	//Copy to the clipboard the selcted text
	void Copy ();

	//Select all the text
	void SelectAll ();
	
	/// Save the content of html widget to a file
	/// <param name="path">The destination file</param>
	bool Save (string path);
	
	/// Enlage the font size in widget
	void ZoomIn ();
	
	/// Shrink the font size in widget
  void ZoomOut ();
  
  /// Reset the font size in widget
  void ZoomReset ();
  
	//Render the HTML code given
	void Render (string html_code);

	// Variable that handles the info encessary for the events
	// As every implementation of HtmlRender will have differents events
	// we try to homogenize them with the variabel
//	string Url { get; }

	Widget HtmlPanel { get; }

  // Print content to the printer
	void Print ();
}


}

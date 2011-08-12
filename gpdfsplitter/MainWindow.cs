using Mono.Unix;
using System;
using Gtk;
using pdfclown = org.pdfclown.files;						// Extern library, pdfclown under GNU Library or Lesser General Public License (LGPL)

public partial class MainWindow: Gtk.Window
{	
	Table table;
	FileChooserButton inputBut;
	Entry outputBut;
	Button done;
	
	RadioButton rb1;
	RadioButton rb2;
	
	SpinButton start;
	SpinButton end;
	
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		Catalog.Init("gpdfsplitter", "/usr/share/locale");		//LOCALIZATION

		
		Title = Catalog.GetString("gPDFsplitter");
		
		table = new Table(4,5,false);
		table.BorderWidth = 5;
		table.Attach( new Label( Catalog.GetString("1. Select the file:") ), 0, 1, 0, 1 );
		table.Attach( new Label( Catalog.GetString("2. Select a range:") ), 0, 1, 1, 3 );
		table.Attach( new Label( Catalog.GetString("3. Output file name:") ), 0, 1, 3, 4);
		table.Attach( new Label( Catalog.GetString("4. Split:") ), 0, 1, 4, 5 );
		
		inputBut = new FileChooserButton("Select file", FileChooserAction.Open);
		inputBut.Filter = new FileFilter();
		inputBut.Filter.AddMimeType( "application/pdf" );
		inputBut.FileSet += OnFileSelect;
		table.Attach( inputBut, 1, 4, 0, 1);
		
		outputBut = new Entry( Catalog.GetString("output.pdf") );
		outputBut.MaxLength = 15;
		outputBut.TooltipText = Catalog.GetString("Directory is home or actual");
		table.Attach( outputBut, 1, 4, 3,4);
		
		done = new Button("done");
		done.Clicked += OnDone;
		table.Attach( done, 1,4,4,5);
		
		start = new SpinButton(0,1,1);
		end = new SpinButton(0,1,1);
		table.Attach( start, 2,3, 1,2);
		table.Attach( end, 3,4, 1,2);
		
		rb1 = new RadioButton( Catalog.GetString("interval") );
		rb2 = new RadioButton( rb1,  Catalog.GetString("chapter") );
		table.Attach( rb1 , 1, 2, 1, 2);
		table.Attach( rb2 , 1, 2, 2, 3);
		
		table.Attach( new Label( Catalog.GetString("Not implemented yet") ), 2,4,2,3 );
		
		Add( table );
		ShowAll();
		
	}
		
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	protected void OnFileSelect (object sender, EventArgs a) 
	{
		pdfclown.File file = new pdfclown.File( ((FileChooserButton)sender).Filename );
		start = new SpinButton( 1, file.Document.Pages.Count, 1 );
		end = new SpinButton( 1, file.Document.Pages.Count, 1 );
		table.Attach( start, 2,3, 1,2);
		table.Attach( end, 3,4, 1,2);
		ShowAll();		
	}
	
	protected void OnDone(object sender, EventArgs a) 
	{
		if( rb2.Active ) return;
		if( start.ValueAsInt > end.ValueAsInt ) return;
		if( inputBut.Filename == null ) return;
		if( inputBut.Filename == string.Empty ) return;
		if( inputBut.Filename == "" ) return;
		if( System.IO.File.Exists( outputBut.Text ) ) {
			MessageDialog md = new MessageDialog(	this, 
													DialogFlags.DestroyWithParent,
													MessageType.Question,
													ButtonsType.YesNo,
													Catalog.GetString("Overwrite output file?") 	);
			md.Modal = true;
			md.Title = Catalog.GetString("Overwrite?");
			
			ResponseType rt = (ResponseType) md.Run();
			if( ResponseType.No == rt ) { md.Destroy(); return; }
			if( ResponseType.Yes == rt ) { md.Destroy(); }
			if( ResponseType.DeleteEvent == rt) { return; }
		}
		
		string command = string.Format("pdftk {0} cat {1}-{2} output {3}",
			inputBut.Filename, start.ValueAsInt, end.ValueAsInt, outputBut.Text);
		
		int ret = Mono.Unix.Native.Syscall.system	( command );	
		if( ret == 0 ) {
			// Sucess
			Application.Quit ();
		} else {
			MessageDialog md = new MessageDialog(	this, 
													DialogFlags.DestroyWithParent,
													MessageType.Error,
													ButtonsType.None,
													Catalog.GetString("Somethink wrong (pdftk return nonzero")	);	
			md.Run();
			md.Destroy();			
		}
	}
}

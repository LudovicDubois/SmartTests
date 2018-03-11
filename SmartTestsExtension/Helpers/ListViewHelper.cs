using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;



namespace SmartTestsExtension.Helpers
{
    public class ListViewHelper
    {
        public static int GetAutoGenerateFromColumn( DependencyObject element )
        {
            if( element == null )
                throw new ArgumentNullException( nameof(element) );

            return (int)element.GetValue( AutoGenerateFromColumnProperty );
        }


        public static void SetAutoGenerateFromColumn( DependencyObject element, int value )
        {
            if( element == null )
                throw new ArgumentNullException( nameof(element) );

            element.SetValue( AutoGenerateFromColumnProperty, value );
        }


        public static readonly DependencyProperty AutoGenerateFromColumnProperty = DependencyProperty.RegisterAttached( "AutoGenerateFromColumn", typeof(int), typeof(ListViewHelper), new FrameworkPropertyMetadata( 0, RaisePropChanged ) );


        private static void RaisePropChanged( DependencyObject obj, DependencyPropertyChangedEventArgs e )
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty( ItemsControl.ItemsSourceProperty, typeof(ListView) );
            descriptor.AddValueChanged( (ListView)obj, ItemsSourceChanged );
        }


        public static readonly DependencyProperty ExcludeColumnsProperty = DependencyProperty.RegisterAttached( "ExcludeColumns", typeof(string), typeof(ListViewHelper), new PropertyMetadata( default(string) ) );


        public static string GetExcludeColumns( UIElement element )
        {
            return (string)element.GetValue( ExcludeColumnsProperty );
        }


        public static void SetExcludeColumns( UIElement element, string value )
        {
            element.SetValue( ExcludeColumnsProperty, value );
        }


        private static void ItemsSourceChanged( object sender, EventArgs e )
        {
            var listView = (ListView)sender;
            var index = GetAutoGenerateFromColumn( listView );
            var columns = ( (GridView)listView.View ).Columns;
            if( index <= 0 )
                columns.Clear();
            else
                while( columns.Count > index )
                    columns.RemoveAt( index );
            var itemsSource = listView.ItemsSource;
            if( itemsSource == null )
                return;
            var enumerator = itemsSource.GetEnumerator();
            if( !enumerator.MoveNext() )
                return;

            var excludedColumns = GetExcludeColumns( listView ).Split( ';' );
            var firstObject = enumerator.Current;
            if( firstObject is DataRowView firstRow )
                SetupColumns( (GridView)listView.View, excludedColumns, firstRow );
            else
                SetupColumns( (GridView)listView.View, excludedColumns, firstObject );
        }


        private static void SetupColumns( GridView gridView, string[] excludedColumns, object firstObject )
        {
            foreach( var property in firstObject.GetType().GetProperties() )
                AddColumn( gridView, excludedColumns, property.Name );
        }


        private static void SetupColumns( GridView gridView, string[] excludedColumns, DataRowView firstRow )
        {
            foreach( DataColumn column in firstRow.Row.Table.Columns )
                AddColumn( gridView, excludedColumns, column.ColumnName );
        }


        private static void AddColumn( GridView gridView, string[] excludedColumns, string columnName )
        {
            if( excludedColumns.Any( column => column == columnName ) )
                return;

            var gridViewColumn = new GridViewColumn { Header = columnName };
            var binding = new Binding( columnName );
            BindingOperations.SetBinding( gridViewColumn, TextBlock.TextProperty, binding );
            gridViewColumn.DisplayMemberBinding = binding;
            gridView.Columns.Add( gridViewColumn );
        }
    }
}
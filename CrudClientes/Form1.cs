using Microsoft.EntityFrameworkCore;

namespace CrudClientes;

public partial class Form1 : Form
{
    // ---------- Controles ----------
    private readonly TextBox txtNombre = new();
    private readonly TextBox txtApellido = new();
    private readonly TextBox txtCorreo = new();
    private readonly TextBox txtBuscarId = new();
    private readonly TextBox txtBuscarNombre = new();
    private readonly Button btnBuscarId = new();
    private readonly Button btnNuevo = new();
    private readonly Button btnGuardar = new();
    private readonly Button btnEditar = new();
    private readonly Button btnEliminar = new();
    private readonly DataGridView dgv = new();

    // Id del cliente seleccionado (0 = cliente nuevo)
    private int _idActual = 0;

    // ---------- Colores ----------
    private static readonly Color ColorFondo = Color.FromArgb(24, 38, 48);
    private static readonly Color ColorCampo = Color.FromArgb(36, 54, 66);
    private static readonly Color ColorAcento = Color.FromArgb(32, 201, 151);

    public Form1()
    {
        InitializeComponent();   // viene del Designer (vacío), lo dejamos
        ConstruirInterfaz();
        Load += (s, e) =>
        {
            CargarClientes();
            ModoEdicion(false);
        };
    }

    // =====================================================
    //  INTERFAZ
    // =====================================================
    private void ConstruirInterfaz()
    {
        Text = "Gestión de Clientes - CRUD con Entity Framework";
        ClientSize = new Size(900, 530);
        MinimumSize = new Size(820, 520);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ColorFondo;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);

        // --- Datos del cliente (izquierda) ---
        var lblTitulo = CrearLabel("DATOS DEL CLIENTE", 20, 20, true);
        var lblNombre = CrearLabel("Nombre:", 20, 60);
        var lblApellido = CrearLabel("Apellido:", 20, 130);
        var lblCorreo = CrearLabel("Correo:", 20, 200);

        EstilizarTextBox(txtNombre, 20, 85, 270);
        EstilizarTextBox(txtApellido, 20, 155, 270);
        EstilizarTextBox(txtCorreo, 20, 225, 270);

        // --- Búsquedas (derecha) ---
        var lblBuscarId = CrearLabel("Buscar por Id:", 320, 20);
        var lblBuscarNombre = CrearLabel("Buscar por nombre / apellido:", 560, 20);

        EstilizarTextBox(txtBuscarId, 320, 45, 150);
        EstilizarBoton(btnBuscarId, "Buscar", 476, 43, 80, 30, ColorAcento);

        EstilizarTextBox(txtBuscarNombre, 560, 45, 300);
        txtBuscarNombre.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // --- Tabla ---
        ConfigurarGrid();

        // --- Botones inferiores ---
        EstilizarBoton(btnNuevo, "Nuevo", 20, 465, 200, 45, ColorAcento);
        EstilizarBoton(btnGuardar, "Guardar", 233, 465, 200, 45, ColorAcento);
        EstilizarBoton(btnEditar, "Editar", 446, 465, 200, 45, Color.FromArgb(52, 120, 180));
        EstilizarBoton(btnEliminar, "Eliminar", 659, 465, 200, 45, Color.FromArgb(200, 70, 70));
        foreach (var b in new[] { btnNuevo, btnGuardar, btnEditar, btnEliminar })
            b.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

        Controls.AddRange(new Control[]
        {
            lblTitulo, lblNombre, lblApellido, lblCorreo,
            txtNombre, txtApellido, txtCorreo,
            lblBuscarId, lblBuscarNombre,
            txtBuscarId, btnBuscarId, txtBuscarNombre,
            dgv,
            btnNuevo, btnGuardar, btnEditar, btnEliminar
        });

        // --- Eventos ---
        btnNuevo.Click += (s, e) => Nuevo();
        btnGuardar.Click += (s, e) => Guardar();
        btnEditar.Click += (s, e) => Editar();
        btnEliminar.Click += (s, e) => Eliminar();
        btnBuscarId.Click += (s, e) => BuscarPorId();
        txtBuscarId.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscarPorId();
                e.SuppressKeyPress = true;
            }
        };
        txtBuscarNombre.TextChanged += (s, e) => CargarClientes(nombre: txtBuscarNombre.Text);
        dgv.SelectionChanged += Dgv_SelectionChanged;
    }

    private void ConfigurarGrid()
    {
        dgv.Left = 320;
        dgv.Top = 90;
        dgv.Width = 540;
        dgv.Height = 350;
        dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.MultiSelect = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.RowHeadersVisible = false;
        dgv.BorderStyle = BorderStyle.None;

        dgv.BackgroundColor = ColorCampo;
        dgv.GridColor = Color.FromArgb(60, 80, 95);
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorAcento;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 25, 30);
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgv.ColumnHeadersHeight = 34;
        dgv.DefaultCellStyle.BackColor = ColorCampo;
        dgv.DefaultCellStyle.ForeColor = Color.White;
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(20, 120, 95);
        dgv.DefaultCellStyle.SelectionForeColor = Color.White;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(30, 46, 58);
        dgv.RowTemplate.Height = 28;
    }

    private static Label CrearLabel(string texto, int x, int y, bool negrita = false) => new()
    {
        Text = texto,
        Left = x,
        Top = y,
        AutoSize = true,
        ForeColor = ColorAcento,
        Font = new Font("Segoe UI", 10, negrita ? FontStyle.Bold : FontStyle.Regular)
    };

    private static void EstilizarTextBox(TextBox t, int x, int y, int ancho)
    {
        t.Left = x;
        t.Top = y;
        t.Width = ancho;
        t.BackColor = ColorCampo;
        t.ForeColor = Color.White;
        t.BorderStyle = BorderStyle.FixedSingle;
        t.Font = new Font("Segoe UI", 11);
    }

    private static void EstilizarBoton(Button b, string texto, int x, int y, int ancho, int alto, Color color)
    {
        b.Text = texto;
        b.Left = x;
        b.Top = y;
        b.Width = ancho;
        b.Height = alto;
        b.BackColor = color;
        b.ForeColor = Color.White;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        b.Cursor = Cursors.Hand;
    }

    // =====================================================
    //  CRUD
    // =====================================================

    // READ: carga los clientes (con filtros opcionales)
    private void CargarClientes(string? nombre = null, int? id = null)
    {
        try
        {
            using var db = new AppDbContext();
            var query = db.Clientes.AsNoTracking().AsQueryable();

            if (id.HasValue)
                query = query.Where(c => c.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(c => c.Nombre.Contains(nombre) || c.Apellido.Contains(nombre));

            dgv.DataSource = query.OrderBy(c => c.Id).ToList();

            if (dgv.Columns["Id"] is DataGridViewColumn colId)
                colId.FillWeight = 30;

            if (dgv.Rows.Count == 0)
            {
                LimpiarCampos();
                ModoEdicion(false);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo conectar con la base de datos:\n\n" + Detalle(ex),
                "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // CREATE: prepara el formulario para un registro nuevo
    private void Nuevo()
    {
        dgv.ClearSelection();
        LimpiarCampos();
        ModoEdicion(true);
        txtNombre.Focus();
    }

    // CREATE + UPDATE: según si _idActual es 0 o no
    private void Guardar()
    {
        var nombre = txtNombre.Text.Trim();
        var apellido = txtApellido.Text.Trim();
        var correo = txtCorreo.Text.Trim();

        if (nombre == "" || apellido == "" || correo == "")
        {
            MessageBox.Show("Completa nombre, apellido y correo.", "Datos incompletos",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!correo.Contains('@') || !correo.Contains('.'))
        {
            MessageBox.Show("El correo no tiene un formato válido.", "Correo inválido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtCorreo.Focus();
            return;
        }

        try
        {
            using var db = new AppDbContext();
            int idGuardado;

            if (_idActual == 0)
            {
                // INSERT
                var nuevo = new Cliente { Nombre = nombre, Apellido = apellido, Correo = correo };
                db.Clientes.Add(nuevo);
                db.SaveChanges();
                idGuardado = nuevo.Id;
            }
            else
            {
                // UPDATE
                var cliente = db.Clientes.Find(_idActual);
                if (cliente == null)
                {
                    MessageBox.Show("El registro ya no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarClientes();
                    return;
                }
                cliente.Nombre = nombre;
                cliente.Apellido = apellido;
                cliente.Correo = correo;
                db.SaveChanges();
                idGuardado = cliente.Id;
            }

            txtBuscarId.Clear();
            txtBuscarNombre.Clear();
            CargarClientes();
            SeleccionarFila(idGuardado);
            MessageBox.Show("Cliente guardado correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al guardar:\n\n" + Detalle(ex), "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // UPDATE: habilita los campos del registro seleccionado
    private void Editar()
    {
        if (_idActual == 0)
        {
            MessageBox.Show("Selecciona un cliente de la tabla para editarlo.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        ModoEdicion(true);
        txtNombre.Focus();
    }

    // DELETE
    private void Eliminar()
    {
        if (_idActual == 0)
        {
            MessageBox.Show("Selecciona un cliente de la tabla para eliminarlo.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirmar = MessageBox.Show(
            $"¿Seguro que deseas eliminar a {txtNombre.Text} {txtApellido.Text}?",
            "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirmar != DialogResult.Yes) return;

        try
        {
            using var db = new AppDbContext();
            var cliente = db.Clientes.Find(_idActual);
            if (cliente != null)
            {
                db.Clientes.Remove(cliente);
                db.SaveChanges();
            }

            _idActual = 0;
            LimpiarCampos();
            CargarClientes();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al eliminar:\n\n" + Detalle(ex), "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // =====================================================
    //  AYUDAS
    // =====================================================
    private void BuscarPorId()
    {
        var texto = txtBuscarId.Text.Trim();
        if (texto == "")
        {
            CargarClientes();
            return;
        }

        if (!int.TryParse(texto, out int id))
        {
            MessageBox.Show("El Id debe ser un número.", "Dato inválido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        CargarClientes(id: id);
        if (dgv.Rows.Count == 0)
            MessageBox.Show($"No existe un cliente con Id {id}.", "Sin resultados",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // Al seleccionar una fila, se llenan los campos
    private void Dgv_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        if (dgv.SelectedRows[0].DataBoundItem is not Cliente c) return;

        _idActual = c.Id;
        txtNombre.Text = c.Nombre;
        txtApellido.Text = c.Apellido;
        txtCorreo.Text = c.Correo;
        ModoEdicion(false);
    }

    private void SeleccionarFila(int id)
    {
        foreach (DataGridViewRow fila in dgv.Rows)
        {
            if (fila.DataBoundItem is Cliente c && c.Id == id)
            {
                dgv.CurrentCell = fila.Cells[0];
                fila.Selected = true;
                break;
            }
        }
    }

    private void LimpiarCampos()
    {
        _idActual = 0;
        txtNombre.Clear();
        txtApellido.Clear();
        txtCorreo.Clear();
    }

    private void ModoEdicion(bool habilitar)
    {
        txtNombre.ReadOnly = !habilitar;
        txtApellido.ReadOnly = !habilitar;
        txtCorreo.ReadOnly = !habilitar;
        btnGuardar.Enabled = habilitar;
        btnGuardar.BackColor = habilitar ? ColorAcento : Color.FromArgb(70, 90, 100);
    }

    private static string Detalle(Exception ex) => ex.InnerException?.Message ?? ex.Message;
}
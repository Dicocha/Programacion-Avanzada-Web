(function ($) {
    'use strict';

    var fmtMoneda = (function () {
        try {
            return new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC' });
        } catch (e) {
            return {
                format: function (valor) { return '₡' + Number(valor).toFixed(2); }
            };
        }
    })();
    
    function post(metodo, datos) {
        return $.ajax({
            type: 'POST',
            url: 'Default.aspx/' + metodo,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(datos || {})
        });
    }

    function unwrap(resp) {
        return (resp && resp.d !== undefined) ? resp.d : resp;
    }

    function mostrarMensaje(texto, exito) {
        var $contenedor = $('#divMensajes');
        $contenedor.empty();

        var $alerta = $('<div/>', {
            'class': 'alert alert-dismissible fade show ' + (exito ? 'alert-success' : 'alert-danger'),
            role: 'alert'
        }).text(texto);

        $alerta.append(
            $('<button/>', {
                type: 'button',
                'class': 'btn-close',
                'data-bs-dismiss': 'alert',
                'aria-label': 'Cerrar'
            })
        );

        $contenedor.append($alerta);
    }

    function errorRed() {
        mostrarMensaje('No se pudo comunicar con el servidor. Verificá la conexión e intentá de nuevo.', false);
    }

    /* ------------------------- Validación (lado cliente) ------------------------- */

    function setError(campo, mensaje) {
        var $campo = $('#' + campo);
        var $error = $('#err' + campo.replace('txt', '').replace('ddl', ''));

        if (mensaje) {
            $campo.addClass('is-invalid').attr('aria-invalid', 'true');
            $error.text(mensaje);
            return false;
        }

        $campo.removeClass('is-invalid').removeAttr('aria-invalid');
        $error.text('');
        return true;
    }

    function normalizarPrecio(valor) {
        return $.trim(valor).replace(/\s/g, '').replace(',', '.');
    }

    function validarNombre() {
        var nombre = $.trim($('#txtNombre').val());
        if (!nombre) return setError('txtNombre', 'El nombre es obligatorio.');
        if (nombre.length > 100) return setError('txtNombre', 'El nombre no puede superar los 100 caracteres.');
        return setError('txtNombre', '');
    }

    function validarPrecio() {
        var crudo = $.trim($('#txtPrecio').val());
        if (!crudo) return setError('txtPrecio', 'El precio es obligatorio.');

        var precio = normalizarPrecio(crudo);
        if (!/^\d{1,7}(\.\d{1,2})?$/.test(precio))
            return setError('txtPrecio', 'Formato inválido: use números con hasta 2 decimales (ej. 1250.50).');

        var valor = parseFloat(precio);
        if (isNaN(valor) || valor <= 0)
            return setError('txtPrecio', 'El precio debe ser mayor a 0.');
        if (valor > 1000000)
            return setError('txtPrecio', 'El precio no puede superar 1,000,000.');

        return setError('txtPrecio', '');
    }

    function validarCantidad() {
        var crudo = $.trim($('#txtCantidad').val());
        if (!crudo) return setError('txtCantidad', 'La cantidad es obligatoria.');
        if (!/^\d{1,7}$/.test(crudo))
            return setError('txtCantidad', 'La cantidad debe ser un número entero sin decimales.');

        var valor = parseInt(crudo, 10);
        if (valor > 1000000)
            return setError('txtCantidad', 'La cantidad no puede superar 1,000,000.');

        return setError('txtCantidad', '');
    }

    function validarCategoria() {
        if (!$('#ddlCategoria').val())
            return setError('ddlCategoria', 'Debe seleccionar una categoría.');
        return setError('ddlCategoria', '');
    }

    /* ------------------------- Formulario ------------------------- */

    function validarFormulario() {
        var vNombre = validarNombre();
        var vPrecio = validarPrecio();
        var vCantidad = validarCantidad();
        var vCategoria = validarCategoria();

        if (!(vNombre && vPrecio && vCantidad && vCategoria)) {
            $('.is-invalid').first().focus();
            return false;
        }
        return true;
    }

    /* ------------------------------ Render en DOM ------------------------------ */

    function filaVacia(columnas, texto) {
        return $('<tr>').append(
            $('<td>', { colspan: columnas, 'class': 'text-center text-muted py-3' }).text(texto)
        );
    }

    function renderTabla(productos, total) {
        var $tbody = $('#tbodyProductos').empty();

        if (!productos || !productos.length) {
            $tbody.append(filaVacia(6, 'No hay productos que coincidan con el filtro. Registrá el primero en el formulario.'));
        } else {
            $.each(productos, function (i, p) {
                var $fila = $('<tr>');
                $fila.append($('<td>').text(p.nombre));
                $fila.append($('<td>').text(p.categoria));
                $fila.append($('<td>', { 'class': 'text-end' }).text(fmtMoneda.format(p.precio)));
                $fila.append($('<td>', { 'class': 'text-center' }).text(p.cantidad));
                $fila.append($('<td>', { 'class': 'text-end' }).text(fmtMoneda.format(p.valorTotal)));
                $fila.append(
                    $('<td>', { 'class': 'text-center' }).append(
                        $('<span>', {
                            'class': 'badge ' + (p.estado === 'Agotado' ? 'bg-danger' : 'bg-success')
                        }).text(p.estado)
                    )
                );
                $tbody.append($fila);
            });
        }

        $('#lblConteo').text(total + ' producto(s) en pantalla.');
    }

    function renderResumen(resumen) {
        $('#kpiProductos').text(resumen.totalProductos);
        $('#kpiUnidades').text(resumen.totalUnidades);
        $('#kpiValor').text(fmtMoneda.format(resumen.valorInventario));
        $('#kpiAgotados').text(resumen.agotados);
        $('#kpis').removeClass('d-none');

        var $tbody = $('#tbodyResumen').empty();

        if (!resumen.porCategoria || !resumen.porCategoria.length) {
            $tbody.append(filaVacia(4, 'Sin datos para mostrar.'));
        } else {
            $.each(resumen.porCategoria, function (i, c) {
                var $fila = $('<tr>');
                $fila.append($('<td>').text(c.categoria));
                $fila.append($('<td>', { 'class': 'text-center' }).text(c.cantidadProductos));
                $fila.append($('<td>', { 'class': 'text-center' }).text(c.unidades));
                $fila.append($('<td>', { 'class': 'text-end' }).text(fmtMoneda.format(c.valorTotal)));
                $tbody.append($fila);
            });
        }

        $('#tablaResumen').removeClass('d-none');
    }

    /* ------------------------------ Acciones Ajax ------------------------------ */

    function cargarCategorias() {
        post('ObtenerCategorias').done(function (respuesta) {
            var datos = unwrap(respuesta);
            if (!datos.ok) return;

            var $registro = $('#ddlCategoria');
            var $filtro = $('#ddlCategoriaFiltro');

            $.each(datos.categorias, function (i, categoria) {
                $registro.append($('<option>', { value: categoria, text: categoria }));
                $filtro.append($('<option>', { value: categoria, text: categoria }));
            });
        }).fail(errorRed);
    }

    /* ------------------------------ Consultar ------------------------------ */

    function consultar() {
        post('Consultar', {
            estado: $('#ddlFiltroEstado').val(),
            categoria: $('#ddlCategoriaFiltro').val()
        }).done(function (respuesta) {
            var datos = unwrap(respuesta);
            if (!datos.ok) {
                mostrarMensaje(datos.mensaje || 'No se pudo consultar el inventario.', false);
                return;
            }
            renderTabla(datos.productos, datos.total);
        }).fail(errorRed);
    }

    function calcularResumen() {
        post('CalcularResumen').done(function (respuesta) {
            var datos = unwrap(respuesta);
            if (!datos.ok) {
                mostrarMensaje(datos.mensaje || 'No se pudo calcular el resumen.', false);
                return;
            }
            renderResumen(datos.resumen);
        }).fail(errorRed);
    }

    function registrar() {
        if (!validarFormulario()) return;

        var $boton = $('#btnRegistrar').prop('disabled', true);

        post('Registrar', {
            nombre: $.trim($('#txtNombre').val()),
            precio: normalizarPrecio($('#txtPrecio').val()),
            cantidad: $.trim($('#txtCantidad').val()),
            categoria: $('#ddlCategoria').val()
        }).done(function (respuesta) {
            var datos = unwrap(respuesta);

            if (!datos.ok) {
                mostrarMensaje(datos.mensaje, false);
                return;
            }

            mostrarMensaje(datos.mensaje, true);
            limpiarFormulario();
            consultar();

            if (!$('#tablaResumen').hasClass('d-none'))
                calcularResumen();
        }).fail(errorRed).always(function () {
            $boton.prop('disabled', false);
        });
    }

    function limpiarFormulario() {
        $('#txtNombre, #txtPrecio, #txtCantidad').val('');
        $('#ddlCategoria').val('');
        setError('txtNombre', '');
        setError('txtPrecio', '');
        setError('txtCantidad', '');
        setError('ddlCategoria', '');
        $('#txtNombre').focus();
    }

    /* ------------------------------- Inicialización ------------------------------- */

    $(function () {
        $('#btnRegistrar').on('click', registrar);
        $('#btnActualizar').on('click', consultar);
        $('#btnCalcularResumen').on('click', calcularResumen);

        $('#ddlFiltroEstado, #ddlCategoriaFiltro').on('change', consultar);

        $('#txtNombre').on('input', validarNombre);
        $('#txtPrecio').on('input', validarPrecio);
        $('#txtCantidad').on('input', validarCantidad);
        $('#ddlCategoria').on('change', validarCategoria);

        $('#txtNombre, #txtPrecio, #txtCantidad').on('keydown', function (evento) {
            if (evento.key === 'Enter') {
                evento.preventDefault();
                $('#btnRegistrar').click();
            }
        });

        cargarCategorias();
        consultar();
    });

})(jQuery);

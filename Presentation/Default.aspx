<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Presentation.Default" ValidateRequest="false" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Control de Inventario</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/Site.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server">
    <nav class="navbar navbar-dark bg-dark">
        <div class="container">
            <span class="navbar-brand mb-0 h1">Control de Inventario</span>
        </div>
    </nav>

    <div class="container py-4">

        <div id="divMensajes" role="alert" aria-live="polite"></div>

        <div class="row">
            <div class="col-lg-5">
                <div class="card mb-4">
                    <div class="card-header bg-white">
                        <strong>1. Registro de producto</strong>
                        <div class="text-muted small">Captura los datos del producto para agregarlo al inventario.</div>
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <label for="txtNombre" class="form-label">Nombre <span class="text-danger">*</span></label>
                            <input type="text" id="txtNombre" class="form-control" maxlength="100"
                                placeholder="Ej. Laptop Lenovo" autocomplete="off" />
                            <div class="invalid-feedback" id="errNombre"></div>
                        </div>
                        <div class="mb-3">
                            <label for="txtPrecio" class="form-label">Precio (CRC) <span class="text-danger">*</span></label>
                            <input type="text" id="txtPrecio" class="form-control"
                                placeholder="Ej. 1250.50" autocomplete="off" inputmode="decimal" />
                            <div class="invalid-feedback" id="errPrecio"></div>
                        </div>
                        <div class="mb-3">
                            <label for="txtCantidad" class="form-label">Cantidad disponible <span class="text-danger">*</span></label>
                            <input type="text" id="txtCantidad" class="form-control"
                                placeholder="Ej. 25" autocomplete="off" inputmode="numeric" />
                            <div class="invalid-feedback" id="errCantidad"></div>
                        </div>
                        <div class="mb-3">
                            <label for="ddlCategoria" class="form-label">Categoría <span class="text-danger">*</span></label>
                            <select id="ddlCategoria" class="form-select">
                                <option value="">Seleccione una categoría...</option>
                            </select>
                            <div class="invalid-feedback" id="errCategoria"></div>
                        </div>
                        <button type="button" id="btnRegistrar" class="btn btn-primary w-100">Registrar producto</button>
                    </div>
                </div>
            </div>

            <div class="col-lg-7">
                <div class="card mb-4">
                    <div class="card-header bg-white">
                        <strong>2. Consulta de productos</strong>
                        <div class="text-muted small">Listado actualizable del inventario en esta sesión.</div>
                    </div>
                    <div class="card-body">
                        <div class="row g-2 mb-3">
                            <div class="col-md-4">
                                <label for="ddlFiltroEstado" class="form-label">Estado</label>
                                <select id="ddlFiltroEstado" class="form-select">
                                    <option value="Todos">Todos</option>
                                    <option value="Disponible">Disponible</option>
                                    <option value="Agotado">Agotado</option>
                                </select>
                            </div>
                            <div class="col-md-5">
                                <label for="ddlCategoriaFiltro" class="form-label">Categoría</label>
                                <select id="ddlCategoriaFiltro" class="form-select">
                                    <option value="Todas">Todas</option>
                                </select>
                            </div>
                            <div class="col-md-3 d-flex align-items-end">
                                <button type="button" id="btnActualizar" class="btn btn-outline-secondary w-100">Actualizar</button>
                            </div>
                        </div>

                        <div id="lblConteo" class="text-muted small d-block mb-2">&nbsp;</div>

                        <div class="table-responsive">
                            <table class="table table-striped table-hover align-middle mb-0" id="tblProductos">
                                <thead class="table-light">
                                    <tr>
                                        <th scope="col">Nombre</th>
                                        <th scope="col">Categoría</th>
                                        <th scope="col" class="text-end">Precio</th>
                                        <th scope="col" class="text-center">Cantidad</th>
                                        <th scope="col" class="text-end">Valor</th>
                                        <th scope="col" class="text-center">Estado</th>
                                    </tr>
                                </thead>
                                <tbody id="tbodyProductos"></tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <div class="card mb-4">
                    <div class="card-header bg-white">
                        <strong>3. Procesamiento contable</strong>
                        <div class="text-muted small">Valor total del inventario y sumatorias por categoría.</div>
                    </div>
                    <div class="card-body">
                        <button type="button" id="btnCalcularResumen" class="btn btn-success mb-3">Calcular resumen del inventario</button>

                        <div id="kpis" class="row text-center g-2 mb-3 d-none">
                            <div class="col-6 col-md-3">
                                <div class="border rounded p-2 h-100">
                                    <div class="text-muted small">Productos</div>
                                    <div class="fs-5 fw-bold" id="kpiProductos">0</div>
                                </div>
                            </div>
                            <div class="col-6 col-md-3">
                                <div class="border rounded p-2 h-100">
                                    <div class="text-muted small">Unidades</div>
                                    <div class="fs-5 fw-bold" id="kpiUnidades">0</div>
                                </div>
                            </div>
                            <div class="col-6 col-md-3">
                                <div class="border rounded p-2 h-100">
                                    <div class="text-muted small">Valor inventario</div>
                                    <div class="fs-5 fw-bold text-success" id="kpiValor">₡0.00</div>
                                </div>
                            </div>
                            <div class="col-6 col-md-3">
                                <div class="border rounded p-2 h-100">
                                    <div class="text-muted small">Agotados</div>
                                    <div class="fs-5 fw-bold text-danger" id="kpiAgotados">0</div>
                                </div>
                            </div>
                        </div>

                        <div id="tablaResumen" class="d-none">
                            <h6>Sumatorias por categoría</h6>
                            <div class="table-responsive">
                                <table class="table table-bordered align-middle mb-0" id="tblResumen">
                                    <thead class="table-light">
                                        <tr>
                                            <th scope="col">Categoría</th>
                                            <th scope="col" class="text-center">Productos</th>
                                            <th scope="col" class="text-center">Unidades</th>
                                            <th scope="col" class="text-end">Valor total</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyResumen"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>

    <script src="Scripts/jquery-3.7.0.min.js"></script>
    <script src="Scripts/bootstrap.bundle.min.js"></script>
    <script src="Scripts/inventario.js"></script>
</body>
</html>

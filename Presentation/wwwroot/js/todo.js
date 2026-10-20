(function ($, window) {
    "use strict";

    const TodoApp = {
        currentEditId: null,

        init: function () {
            this.bindEvents();
            this.loadTodos();
        },

        bindEvents: function () {
            const self = this;

            $("#btn-new").on("click", function () {
                self.openCreateModal();
            });

            $("#todo-save").on("click", function () {
                self.saveTodo();
            });

            $("#todo-modal").on("shown.bs.modal", function () {
                $("#todo-title-input").focus();
            });

            $("#todo-list").on("click", ".btn-toggle", function (e) {
                e.preventDefault();
                const id = $(this).closest("li").data("id");
                self.toggle(id);
            });

            $("#todo-list").on("click", ".btn-edit", function (e) {
                e.preventDefault();
                const id = $(this).closest("li").data("id");
                self.openEditModal(id);
            });
        },

        loadTodos: function () {
            const self = this;
            $.get("/Todo/List")
                .done(function (data) {
                    self.render(data);
                })
                .fail(function () {
                    console.error("Error cargando tareas");
                });
        },

        render: function (items) {
            const $list = $("#todo-list");
            $list.empty();

            if (!items || items.length === 0) {
                $list.append('<li class="list-group-item text-muted">Sin tareas</li>');
                return;
            }

            const self = this;
            items.forEach(function (item) {
                const checked = item.isCompleted ? "checked" : "";
                const textClass = item.isCompleted ? "text-decoration-line-through text-muted" : "";
                const descriptionHtml = item.description
                    ? `<br><span class="ms-4 ${textClass}">${self.escapeHtml(item.description)}</span>`
                    : "";
                const html = `
                    <li class="list-group-item d-flex justify-content-between align-items-center" data-id="${self.escapeAttr(item.id)}">
                        <div class="form-check flex-grow-1">
                            <input class="form-check-input btn-toggle" type="checkbox" ${checked}>
                            <span class="ms-2 ${textClass}">${self.escapeHtml(item.title)}</span>${descriptionHtml}
                        </div>
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-secondary btn-edit">Editar</button>
                        </div>
                    </li>`;
                $list.append(html);
            });
        },

        openCreateModal: function () {
            this.currentEditId = null;
            $("#todo-modal-title").text("Nueva tarea");
            $("#todo-title-input").val("");
            $("#todo-description-input").val("");
            $("#todo-modal").modal("show");
        },

        openEditModal: function (id) {
            this.currentEditId = id;
            $("#todo-modal-title").text("Editar tarea");
            $.get("/Todo/Get/" + id)
                .done(function (todo) {
                    $("#todo-title-input").val(todo.title || "");
                    $("#todo-description-input").val(todo.description || "");
                    $("#todo-modal").modal("show");
                })
                .fail(function () {
                    console.error("Error cargando tarea");
                });
        },

        saveTodo: function () {
            const title = $("#todo-title-input").val().trim();
            const description = $("#todo-description-input").val().trim();
            if (!title) {
                return;
            }
            const self = this;
            if (this.currentEditId === null) {
                $.ajax({
                    url: "/Todo/Create",
                    method: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ title: title, description: description })
                }).done(function () {
                    $("#todo-modal").modal("hide");
                    self.loadTodos();
                }).fail(function () {
                    console.error("Error creando tarea");
                });
            } else {
                $.ajax({
                    url: "/Todo/Edit",
                    method: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ id: self.currentEditId, title: title, description: description })
                }).done(function () {
                    $("#todo-modal").modal("hide");
                    self.loadTodos();
                }).fail(function () {
                    console.error("Error editando tarea");
                });
            }
        },

        toggle: function (id) {
            const self = this;
            $.ajax({
                url: "/Todo/Toggle",
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify({ id: id })
            }).done(function () {
                self.loadTodos();
            }).fail(function () {
                console.error("Error cambiando estado");
            });
        },

        escapeHtml: function (text) {
            const div = document.createElement("div");
            div.textContent = text;
            return div.innerHTML;
        },

        escapeAttr: function (text) {
            return String(text).replace(/"/g, "&quot;");
        }
    };

    $(document).ready(function () {
        TodoApp.init();
    });

})(jQuery, window);

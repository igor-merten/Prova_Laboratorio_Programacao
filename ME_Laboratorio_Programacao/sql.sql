CREATE DATABASE "AgentMM"
    WITH ENCODING = 'UTF8'
    LC_COLLATE = 'pt_BR.UTF-8'
    LC_CTYPE   = 'pt_BR.UTF-8'
    TEMPLATE   = template0;

\c "AgentMM"

CREATE TABLE "PerfilAcesso" (
    "Id"          SERIAL       PRIMARY KEY,
    "Nome"        VARCHAR(255) NOT NULL DEFAULT '',
    "Descricao"   VARCHAR(500) NOT NULL DEFAULT '',
    "DataCriacao" TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE "Usuarios" (
    "Id"             SERIAL       PRIMARY KEY,
    "Nome"           VARCHAR(255) NOT NULL DEFAULT '',
    "Email"          VARCHAR(320) NOT NULL DEFAULT '',
    "Senha"          VARCHAR(500) NOT NULL DEFAULT '',
    "Ativo"          BOOLEAN      NOT NULL DEFAULT TRUE,
    "PerfilAcessoId" INT          NOT NULL,
    "DataCriacao"    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    CONSTRAINT "UQ_Usuarios_Email"
        UNIQUE ("Email"),

    CONSTRAINT "FK_Usuarios_PerfilAcesso"
        FOREIGN KEY ("PerfilAcessoId")
        REFERENCES "PerfilAcesso" ("Id")
        ON DELETE RESTRICT
);

CREATE TABLE "CategoriaAgente" (
    "Id"     SERIAL       PRIMARY KEY,
    "Nome"   VARCHAR(255) NOT NULL DEFAULT '',
    "CorHex" VARCHAR(7)   NOT NULL DEFAULT '#ffffff'
);

CREATE TABLE "Agente" (
    "Id"               SERIAL       PRIMARY KEY,
    "TipoAgente"       VARCHAR(50)  NOT NULL,   -- 'Padrao' | 'Super'
    "Nome"             VARCHAR(255) NOT NULL,
    "Descricao"        TEXT,
    "CategoriaAgenteId" INT         NOT NULL,
    "Ativo"            BOOLEAN      NOT NULL DEFAULT TRUE,
    "DataCriacao"      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    CONSTRAINT "CK_Agente_TipoAgente"
        CHECK ("TipoAgente" IN ('Padrao', 'Super')),

    CONSTRAINT "FK_Agente_CategoriaAgente"
        FOREIGN KEY ("CategoriaAgenteId")
        REFERENCES "CategoriaAgente" ("Id")
        ON DELETE RESTRICT
);

CREATE TABLE "CanalOrigem" (
    "Id"          SERIAL       PRIMARY KEY,
    "Nome"        VARCHAR(255) NOT NULL,
    "Ativo"       BOOLEAN      NOT NULL DEFAULT TRUE,
    "DataCriacao" TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE "SessaoAtendimento" (
    "Id"             SERIAL      PRIMARY KEY,
    "UsuarioId"      INT         NOT NULL,
    "AgenteId"       INT         NOT NULL,
    "CanalOrigemId"  INT         NOT NULL,
    "DataInicio"     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "DataFim"        TIMESTAMPTZ,
    "Status"         VARCHAR(50) NOT NULL DEFAULT 'Aberta',

    CONSTRAINT "FK_Sessao_Usuario"
        FOREIGN KEY ("UsuarioId")
        REFERENCES "Usuarios" ("Id"),

    CONSTRAINT "FK_Sessao_Agente"
        FOREIGN KEY ("AgenteId")
        REFERENCES "Agente" ("Id"),

    CONSTRAINT "FK_Sessao_CanalOrigem"
        FOREIGN KEY ("CanalOrigemId")
        REFERENCES "CanalOrigem" ("Id")
);

CREATE TABLE "Mensagem" (
    "Id"                  SERIAL       PRIMARY KEY,
    "SessaoAtendimentoId" INT          NOT NULL,
    "Remetente"           VARCHAR(50)  NOT NULL DEFAULT '',
    "Conteudo"            TEXT         NOT NULL DEFAULT '',
    "EnviadaEm"           TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    CONSTRAINT "FK_Mensagem_Sessao"
        FOREIGN KEY ("SessaoAtendimentoId")
        REFERENCES "SessaoAtendimento" ("Id")
        ON DELETE CASCADE
);

CREATE TABLE "ContextoMemoria" (
    "Id"               SERIAL      PRIMARY KEY,
    "AgenteId"         INT         NOT NULL,
    "UsuarioId"        INT         NOT NULL,
    "Resumo"           TEXT,
    "UltimaAtualizacao" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT "FK_Contexto_Agente"
        FOREIGN KEY ("AgenteId")
        REFERENCES "Agente" ("Id"),

    CONSTRAINT "FK_Contexto_Usuario"
        FOREIGN KEY ("UsuarioId")
        REFERENCES "Usuarios" ("Id")
);

CREATE TABLE "LogAuditoria" (
    "Id"          SERIAL       PRIMARY KEY,
    "UsuarioId"   INT,
    "Acao"        VARCHAR(500) NOT NULL,
    "Entidade"    VARCHAR(255),
    "DataCriacao" TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    CONSTRAINT "FK_Log_Usuario"
        FOREIGN KEY ("UsuarioId")
        REFERENCES "Usuarios" ("Id")
        ON DELETE SET NULL
);

CREATE TABLE "EstatisticaAcesso" (
    "Id"             SERIAL      PRIMARY KEY,
    "AgenteId"       INT         NOT NULL,
    "CanalOrigemId"  INT         NOT NULL,
    "TotalSessoes"   INT         NOT NULL DEFAULT 0,
    "TotalMensagens" INT         NOT NULL DEFAULT 0,
    "DataCriacao"    TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT "FK_Estatistica_Agente"
        FOREIGN KEY ("AgenteId")
        REFERENCES "Agente" ("Id"),

    CONSTRAINT "FK_Estatistica_CanalOrigem"
        FOREIGN KEY ("CanalOrigemId")
        REFERENCES "CanalOrigem" ("Id")
);


INSERT INTO "PerfilAcesso" ("Nome", "Descricao") VALUES
  ('Admin', 'Acesso total ao sistema'),
  ('Operador', 'Acesso ao painel e chat');

INSERT INTO "Usuarios" ("Nome", "Email", "Senha", "Ativo", "PerfilAcessoId") VALUES
  ('Administrador', 'admin@teste.com', '$2a$11$RtiDLb20u3bYdbp5bSBe1OR7XCV4r8b5ROdQgnQmAFSA/q8xiwgnS', true, 1),
  ('Operador', 'operador@teste.com', '$2a$11$RtiDLb20u3bYdbp5bSBe1OR7XCV4r8b5ROdQgnQmAFSA/q8xiwgnS', true, 2);

INSERT INTO "CategoriaAgente" ("Nome", "CorHex") VALUES
  ('Vendas', '#6366f1'),
  ('Suporte', '#22c55e'),
  ('Financeiro', '#f59e0b'),
  ('RH', '#ec4899');

INSERT INTO "Agente" ("TipoAgente", "Nome", "Descricao", "CategoriaAgenteId", "Ativo") VALUES
  ('Padrao', 'Agente de Vendas', 'Especialista em conversão de leads', 1, true),
  ('Super', 'Agente de Suporte', 'Suporte técnico ao cliente', 2, true),
  ('Super', 'Agente Financeiro', 'Consultas e negociação financeira', 3, true),
  ('Padrao', 'Agente de RH', 'Recrutamento e gestão de pessoas', 4, true);

INSERT INTO "CanalOrigem" ("Nome", "Ativo") VALUES
  ('Site Principal', true),
  ('Loja Virtual', true),
  ('App Mobile', true);

-- CREATE INDEX "IX_Usuarios_PerfilAcessoId"         ON "Usuarios"          ("PerfilAcessoId");
-- CREATE INDEX "IX_Agente_CategoriaAgenteId"        ON "Agente"            ("CategoriaAgenteId");
-- CREATE INDEX "IX_Agente_TipoAgente"               ON "Agente"            ("TipoAgente");
-- CREATE INDEX "IX_Sessao_UsuarioId"                ON "SessaoAtendimento" ("UsuarioId");
-- CREATE INDEX "IX_Sessao_AgenteId"                 ON "SessaoAtendimento" ("AgenteId");
-- CREATE INDEX "IX_Sessao_CanalOrigemId"            ON "SessaoAtendimento" ("CanalOrigemId");
-- CREATE INDEX "IX_Mensagem_SessaoAtendimentoId"    ON "Mensagem"          ("SessaoAtendimentoId");
-- CREATE INDEX "IX_Contexto_AgenteId"               ON "ContextoMemoria"   ("AgenteId");
-- CREATE INDEX "IX_Contexto_UsuarioId"              ON "ContextoMemoria"   ("UsuarioId");
-- CREATE INDEX "IX_Log_UsuarioId"                   ON "LogAuditoria"      ("UsuarioId");
-- CREATE INDEX "IX_Estatistica_AgenteId"            ON "EstatisticaAcesso" ("AgenteId");
-- CREATE INDEX "IX_Estatistica_CanalOrigemId"       ON "EstatisticaAcesso" ("CanalOrigemId");
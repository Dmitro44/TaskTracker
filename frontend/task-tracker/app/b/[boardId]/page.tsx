"use client";

import React, { useEffect, useMemo, useState } from "react";
import { Box, Flex, Heading, Text, useDisclosure, useToast } from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";
import { getFullBoard, BoardFull } from "../../services/boards";
import { Column, ColumnFull, createColumn, moveColumn, updateColumn } from "../../services/columns";
import {Card, createCard, moveCard} from "../../services/cards";
import { CreateColumnModal } from "../../components/board/CreateColumnModal";
import { CreateCardModal } from "../../components/board/CreateCardModal";
import { ColumnBox } from "../../components/board/ColumnBox";

import { DndContext, DragEndEvent, DragOverEvent, DragOverlay, DragStartEvent, PointerSensor, useSensor, useSensors } from "@dnd-kit/core"
import { arrayMove, SortableContext } from "@dnd-kit/sortable"
import { createPortal } from "react-dom";
import { CardBox } from "../../components/board/CardBox";

export default function BoardPage({ params }: { params: Promise<{ boardId: string }> }) {
  const [board, setBoard] = useState<BoardFull | null>(null);
  const [columns, setColumns] = useState<Column[]>([]);
  const [cards, setCards] = useState<Card[]>([]);
  const [loading, setLoading] = useState(true);
  

  const columnsId = useMemo(() => columns?.map((col) => col.id) ?? [], [columns])
  const [activeColumn, setActiveColumn] = useState<ColumnFull | null>(null);

  const [activeCard, setActiveCard] = useState<Card | null>(null);

  const sensors = useSensors(useSensor(PointerSensor, {
    activationConstraint: {
      distance: 3,
    },
  }));
  
  // Column modal state
  const [newColTitle, setNewColTitle] = useState("");
  const { isOpen, onOpen, onClose } = useDisclosure();
  const toast = useToast();

  // Card modal state
  const [cardModalColumnId, setCardModalColumnId] = useState<string | null>(null);
  const [newCardTitle, setNewCardTitle] = useState("");
  const {
    isOpen: isCardModalOpen,
    onOpen: onCardModalOpen,
    onClose: onCardModalClose,
  } = useDisclosure();

  const { boardId } = React.use(params);

  // Загрузка доски
  const fetchBoard = async () => {
    setLoading(true);
    try {
      const data = await getFullBoard(boardId);

      const allCards = data.columns.flatMap(col => (col.cards ?? []).map(card => ({
        ...card,
      })));

      const sortedColumns = data.columns
          .map((col) => ({
            id: col.id,
            title: col.title,
            position: col.position,
            boardId: col.boardId
          }))
          .sort((a, b) => a.position - b.position);

      setBoard(data);
      setColumns(sortedColumns);
      setCards(allCards);
    } catch {
      toast({ title: "Ошибка загрузки доски", status: "error" });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBoard();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [boardId]);

  // Добавление колонки
  const handleAddColumn = async () => {
    if (!newColTitle.trim()) return;

    // 1. Создаем временную колонку
    const tempId = "temp-" + Date.now();
    const optimisticColumn = {
      id: tempId,
      title: newColTitle,
      position: columns.length,
      boardId: boardId,
    };

    setColumns(prev => [...prev, optimisticColumn]);
    setNewColTitle("");
    onClose();

    try {
      // 2. Запрос на создание колонки на бэкенде
      const createdColumn = await createColumn({
        title: newColTitle,
        position: optimisticColumn.position,
        boardId: boardId
      });

      // 3. Заменяем временную колонку на настоящую (по id)
      setColumns(prev =>
        prev.map(col =>
          col.id === tempId ? createdColumn : col
        )
      );
    } catch {
      // 4. При ошибке удаляем временную колонку
      setColumns(prev => prev.filter(col => col.id !== tempId));
      toast({ title: "Ошибка при создании колонки", status: "error" });
    }
  };

  // Добавление карточки
  const handleAddCardClick = (columnId: string) => {
    setCardModalColumnId(columnId);
    setNewCardTitle("");
    onCardModalOpen();
  };
  const handleAddCard = async () => {
    if (!columns || !cardModalColumnId || !newCardTitle.trim()) return;

    // временная карточка
    const tempId = "temp-" + Date.now();
    const optimisticCard = {
      id: tempId,
      title: newCardTitle,
      position: cards.filter(c => c.columnId === cardModalColumnId).length,
      columnId: cardModalColumnId,
    };

    setCards(prev => [...prev, optimisticCard]);

    setNewCardTitle("");
    setCardModalColumnId(null);
    onCardModalClose();

    try {
      const createdCard = await createCard({
        title: newCardTitle,
        position: optimisticCard.position,
        columnId: cardModalColumnId,
      });

      // Обнови временную карточку на настоящую (по id)
      setCards(prev => 
        prev.map(card => card.id === tempId ? createdCard : card)
      );
    } catch {
      // Удали временную карточку при ошибке
      setCards(prev => prev.filter(card => card.id !== tempId));
      toast({ title: "Ошибка при создании карточки", status: "error" });
    }
  };

  if (loading) {
    return (
      <Flex height="100vh" align="center" justify="center">
        <Text>Загрузка...</Text>
      </Flex>
    );
  }

  if (!board) {
    return (
      <Flex height="100vh" align="center" justify="center">
        <Text>Ошибка загрузки доски</Text>
      </Flex>
    );
  }

  return (
      <Flex direction="column" h="100vh" overflow="hidden">
        <Flex align="center" justify="space-between" p={6} borderBottom="1px solid" borderColor="gray.200" bg="white">
          <Heading size="lg">{board.title}</Heading>
        </Flex>
        <Box flex="1" overflowX="auto" p={6} bg="gray.50">
          <DndContext 
            sensors={sensors}
            onDragStart={onDragStart} 
            onDragEnd={onDragEnd}
            onDragOver={onDragOver}
          >
            <SortableContext items={columnsId}>
              <Flex gap={6} align="flex-start">
                  {columns.map((column) => (
                    <ColumnBox
                      key={column.id}
                      column={column}
                      cards={cards.filter(c => c.columnId === column.id)}
                      onAddCard={handleAddCardClick}
                      updateColumn={handleUpdateColumn}
                    />
                  ))}
                <Box
                  minW="240px"
                  bg="blue.50"
                  borderRadius="md"
                  boxShadow="md"
                  p={4}
                  display="flex"
                  flexDirection="column"
                  alignItems="center"
                  justifyContent="center"
                  maxH="80vh"
                  userSelect="none"
                  opacity={0.5}
                  cursor="pointer"
                  transition="all 0.2s"
                  _hover={{ opacity: 0.85, bg: "blue.100" }}
                  onClick={onOpen}
                >
                  <AddIcon boxSize={6} mb={2} color="blue.500" />
                  <Text fontSize="md" color="blue.600">
                    Добавить колонку
                  </Text>
                </Box>
              </Flex>
            </SortableContext>

            {createPortal(
              <DragOverlay>
                {activeColumn &&
                  <ColumnBox
                    column={activeColumn}
                    cards={cards.filter((c) => c.columnId === activeColumn.id)}
                    onAddCard={()=>{}}
                    updateColumn={()=>{}}
                  />
                }

                {activeCard &&
                  <CardBox
                    card={activeCard}
                  />
                }
              </DragOverlay>,
              document.body
            )}
          </DndContext>
        </Box>

        {/* Модалка создания новой колонки */}
        <CreateColumnModal
          isOpen={isOpen}
          onClose={onClose}
          newColTitle={newColTitle}
          setNewColTitle={setNewColTitle}
          onCreate={handleAddColumn}
        />

        {/* Модалка создания новой карточки */}
        <CreateCardModal
          isOpen={isCardModalOpen}
          onClose={onCardModalClose}
          newCardTitle={newCardTitle}
          setNewCardTitle={setNewCardTitle}
          onCreate={handleAddCard}
        />
      </Flex>
  );

  function onDragStart(event: DragStartEvent) {
    console.log(event);
    if (event.active.data.current?.type === "Column") {
      setActiveColumn(event.active.data.current.column);
      return;
    }

    if (event.active.data.current?.type === "Card") {
      setActiveCard(event.active.data.current.card)
      return;
    }
  }

  function onDragEnd(event: DragEndEvent) {
    setActiveColumn(null);
    setActiveCard(null);

    const { active, over } = event;

    if (!over) return;

    if (active.data.current?.type === "Column" && over.data.current?.type === "Column") {
      const activeId = active.id;
      const overId = over.id;

      if (activeId === overId) return;

      setColumns((columns) => {
        const activeColumnIndex = columns.findIndex(
          (col) => col.id === activeId
        );

        const overColumnIndex = columns.findIndex(
          (col) => col.id === overId
        );

        moveColumn({ columnId: activeId as string, position: overColumnIndex});

        return arrayMove(columns, activeColumnIndex, overColumnIndex);
      })
    }
    else if (active.data.current?.type === "Card") {
      const activeId = active.id;

      if (over.data.current?.type === "Card") {
        const overId = over.id;
        const overCard = cards.find(c => c.id === overId);

        if (!overCard) return;

        const overCardPosition = cards
            .filter(card => card.columnId === overCard.columnId)
            .findIndex(card => card.id === overCard.id);

        setCards(prevCards => {
          const activeIndex = prevCards.findIndex(c => c.id === activeId);
          const overIndex = prevCards.findIndex(c => c.id === overId);


          const newCards = [...prevCards];
          newCards[activeIndex] = {
            ...newCards[activeIndex],
            columnId: cards[overIndex].columnId
          };

          return arrayMove(newCards, activeIndex, overIndex);
        })

        moveCard({cardId: activeId.toString(), toColumnId: overCard.columnId, position: overCardPosition});
      }
      else if (over.data.current?.type === "Column") {
        const overColumnId = over.id;

        setCards((cards) => {
          const activeIndex = cards.findIndex((c) => c.id === activeId)

          cards[activeIndex].columnId = overColumnId.toString();

          return arrayMove(cards, activeIndex, activeIndex);
        })

        const newPosition = cards.filter(c => c.columnId === overColumnId).length;

        moveCard({
          cardId: activeId.toString(),
          toColumnId: overColumnId.toString(),
          position: newPosition,
        })
      }
    }
  }

  function onDragOver(event: DragOverEvent) {
    const { active, over }  = event;

    if (!over) return;

    const activeId = active.id;
    const overId = over.id;

    if (activeId === overId) return;

    const isActiveACard = active.data.current?.type === "Card"
    const isOverACard = over.data.current?.type === "Card"

    if (!isActiveACard) return;

    // Dropping card over another card
    if (isActiveACard && isOverACard) {
      setCards((cards) => {
        const activeIndex = cards.findIndex((c) => c.id === activeId)

        const overIndex = cards.findIndex((c) => c.id === overId);

        cards[activeIndex].columnId = cards[overIndex].columnId;

        return arrayMove(cards, activeIndex, overIndex);
      });
    }

    const isOverAColumn = over.data.current?.type === "Column"

    // dropping card over column
    if (isActiveACard && isOverAColumn) {
      setCards((cards) => {
        const activeIndex = cards.findIndex((c) => c.id === activeId)
        const overIndex = cards.findIndex((c) => c.id === overId)

        const newCards = [...cards];

        newCards[activeIndex] = {
          ...newCards[activeIndex],
          columnId: overId.toString(),
        };

        return arrayMove(newCards, activeIndex, overIndex);
      })
    }
  }

  function handleUpdateColumn(columnId: string, title: string) {
    const newColumns = columns.map((col) => {
      if (col.id !== columnId) return col;
      return {...col, title};
    });

    setColumns(newColumns);
    updateColumn({ columnId, title });
  }
}

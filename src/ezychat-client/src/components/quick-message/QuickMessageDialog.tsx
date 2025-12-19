import React, { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { quickMessageService } from "@/services/quickMessageService";
import { QuickMessage, CreateQuickMessageDto } from "@/types";
import { toast } from "sonner";
import { Plus } from "lucide-react";
import { QuickMessageForm } from "./QuickMessageForm";
import { QuickMessageList } from "./QuickMessageList";

interface QuickMessageDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSelectMessage?: (content: string) => void;
}

export const QuickMessageDialog: React.FC<QuickMessageDialogProps> = ({
  open,
  onOpenChange,
  onSelectMessage,
}) => {
  const [quickMessages, setQuickMessages] = useState<QuickMessage[]>([]);
  const [loading, setLoading] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [formData, setFormData] = useState({ key: "", content: "" });
  const [errors, setErrors] = useState({ key: "", content: "" });

  useEffect(() => {
    if (open) {
      loadQuickMessages();
    }
  }, [open]);

  const loadQuickMessages = async () => {
    try {
      setLoading(true);
      const messages = await quickMessageService.getUserQuickMessages();
      setQuickMessages(messages);
    } catch (error) {
      toast.error("Failed to load quick messages");
    } finally {
      setLoading(false);
    }
  };

  const validateForm = (): boolean => {
    const newErrors = { key: "", content: "" };
    let isValid = true;

    if (!formData.key.trim()) {
      newErrors.key = "Key is required";
      isValid = false;
    } else if (!/^[a-zA-Z0-9_-]+$/.test(formData.key)) {
      newErrors.key =
        "Key can only contain letters, numbers, underscores, and hyphens";
      isValid = false;
    } else if (formData.key.length > 50) {
      newErrors.key = "Key cannot exceed 50 characters";
      isValid = false;
    }

    if (!formData.content.trim()) {
      newErrors.content = "Content is required";
      isValid = false;
    } else if (formData.content.length > 10000) {
      newErrors.content = "Content cannot exceed 10 000 characters";
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleCreate = async () => {
    if (!validateForm()) return;

    try {
      const dto: CreateQuickMessageDto = {
        key: formData.key.trim(),
        content: formData.content.trim(),
      };
      const newMessage = await quickMessageService.createQuickMessage(dto);
      setQuickMessages([...quickMessages, newMessage]);
      setFormData({ key: "", content: "" });
      setIsCreating(false);
      toast.success("Quick message created successfully");
    } catch (error: any) {
      toast.error(error.message || "Failed to create quick message");
    }
  };

  const handleUpdate = async () => {
    if (!editingId || !validateForm()) return;

    try {
      const dto = {
        id: editingId,
        key: formData.key.trim(),
        content: formData.content.trim(),
      };
      const updated = await quickMessageService.updateQuickMessage(
        editingId,
        dto
      );
      setQuickMessages(
        quickMessages.map((msg) => (msg.id === editingId ? updated : msg))
      );
      setEditingId(null);
      setFormData({ key: "", content: "" });
      toast.success("Quick message updated successfully");
    } catch (error: any) {
      toast.error(error.message || "Failed to update quick message");
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this quick message?")) return;

    try {
      await quickMessageService.deleteQuickMessage(id);
      setQuickMessages(quickMessages.filter((msg) => msg.id !== id));
      toast.success("Quick message deleted successfully");
    } catch (error) {
      toast.error("Failed to delete quick message");
    }
  };

  const handleEdit = (message: QuickMessage) => {
    setEditingId(message.id);
    setFormData({ key: message.key, content: message.content });
    setIsCreating(false);
    setErrors({ key: "", content: "" });
  };

  const handleCancelEdit = () => {
    setEditingId(null);
    setIsCreating(false);
    setFormData({ key: "", content: "" });
    setErrors({ key: "", content: "" });
  };

  const handleUseMessage = (content: string) => {
    if (onSelectMessage) {
      onSelectMessage(content);
      onOpenChange(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-3xl max-h-[80vh]">
        <DialogHeader>
          <DialogTitle>Quick Messages</DialogTitle>
          <DialogDescription>
            Create and manage quick messages for faster responses
          </DialogDescription>
        </DialogHeader>

        <div className="flex flex-col gap-4">
          {/* Create/Edit Form */}
          {(isCreating || editingId) && (
            <QuickMessageForm
              mode={editingId ? "edit" : "create"}
              formData={formData}
              errors={errors}
              onFormDataChange={setFormData}
              onSubmit={editingId ? handleUpdate : handleCreate}
              onCancel={handleCancelEdit}
            />
          )}

          {/* Add New Button */}
          {!isCreating && !editingId && (
            <>
              <Button
                onClick={() => setIsCreating(true)}
                className="w-full"
                variant="outline"
              >
                <Plus className="w-4 h-4 mr-2" />
                Add New Quick Message
              </Button>
              <QuickMessageList
                messages={quickMessages}
                loading={loading}
                onUseMessage={onSelectMessage ? handleUseMessage : undefined}
                onEdit={handleEdit}
                onDelete={handleDelete}
              />
            </>
          )}

          {/* Quick Messages List */}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Close
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
